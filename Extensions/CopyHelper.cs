using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions
{
    public delegate void ProgressChangeDelegate(long copiedSize, long totalSize, double progress);
    public delegate void CompleteDelegate();

    public class DirectoryRoboCopy
    {
        class FileProgressInfo
        {
            public readonly string FilePath;
            public long CopiedSize = 0;
            public readonly long TotalSize;

            public FileProgressInfo(string filePath)
            {
                FilePath = filePath;
                TotalSize = new FileInfo(filePath).Length;
            }
        }

        private readonly string _sourcePath;
        private readonly string _targetPath;

        private readonly List<FileProgressInfo> _sourceFiles;
        public readonly long TotalSize;

        public long TotalCopiedSize = 0;
        public double TotalProgress => ((double)TotalCopiedSize) / TotalSize;

        public event ProgressChangeDelegate OnProgressChanged;
        public event CompleteDelegate OnComplete;

        public DirectoryRoboCopy(string sourcePath, string targetPath)
        {
            _sourcePath = sourcePath;
            _targetPath = targetPath;

            _sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Select(x => new FileProgressInfo(x))
                .ToList();
            TotalSize = _sourceFiles.Sum(x => x.TotalSize);
        }

        public void Copy()
        {
            const int bufferSize = 1024 * 1024;
            var buffer = new byte[bufferSize];

            if (Directory.Exists(_targetPath))
            {
                Directory.Delete(_targetPath, true);
            }

            using (var p = new Process())
            {
                p.StartInfo.Arguments = @"/C ROBOCOPY ""{0}"" ""{1}"" /S /mt:16".With(_sourcePath, _targetPath);
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;

                var currentDirectory = string.Empty;
                var currentFile = string.Empty;
                var prevCopiedSize = 0L;
                var newDirTexts = new[] { "New Dir", "새 디렉터리" };
                var newFileTexts = new[] { "New File", "새 파일" };

                p.OutputDataReceived += (sender, args) =>
                {
                    var text = args?.Data?.Trim();
                    if (text == null)
                    {
                        return;
                    }
                    
                    // text 는 4가지 경우로 나뉘어 처리된다.
                    // 1. directory가 변경된 경우
                    // 2. 100% 완료 된 경우
                    // 3. file이 변경된 경우
                    // 4. progress 가 진행된 경우

                    if (newDirTexts.Any(x => text.StartsWith(x)))
                    {
                        // text = "New Dir          5    C:\BroServ\certs\"
                        // text = "새 디렉터리        3  D:\xbox 관련\utils\June_QFE8\"
                        currentDirectory = text.Substring(text.IndexOf(_sourcePath));
                    }
                    else if (text.StartsWith("100%"))
                    {
                        if (text == "100%")
                        {
                            if (string.IsNullOrEmpty(currentFile))
                            {
                                // 한 파일에 대해서 100% 가 여러번 나오는 경우가 있다. 반올림 해서 그렇게 되는것 같다.
                                return;
                            }
                            var filePath = Path.Combine(currentDirectory, currentFile);
                            var fileInfo = _sourceFiles.First(x => x.FilePath == filePath);
                            if (fileInfo != null)
                            {
                                Interlocked.Add(ref TotalCopiedSize, fileInfo.TotalSize - prevCopiedSize);
                                OnProgressChanged?.Invoke(TotalCopiedSize, TotalSize, TotalProgress);
                            }
                        }
                        else
                        {
                            // text = "100%        New File                1422        ca-cert.pem"
                            // text = "100%        New File               1.0 m        AWSSDK.EC2.dll"
                            currentFile = text.Split('\t').Last();
                            var filePath = Path.Combine(currentDirectory, currentFile);
                            var fileInfo = _sourceFiles.First(x => x.FilePath == filePath);
                            if (fileInfo != null)
                            {
                                fileInfo.CopiedSize = fileInfo.TotalSize;
                                Interlocked.Add(ref TotalCopiedSize, fileInfo.TotalSize);
                                OnProgressChanged?.Invoke(TotalCopiedSize, TotalSize, TotalProgress);
                            }
                        }

                        currentFile = string.Empty;
                        prevCopiedSize = 0;
                    }
                    else if (newFileTexts.Any(x => text.StartsWith(x)))
                    {
                        // text = "새 파일        379.2 m  XboxOneXDKTools_06_2017_qfe8.zip"
                        // text = "새 파일            138  ClientAWS PerfBot.bat"
                        currentFile = text.Split('\t').Last();
                    }
                    else if (text.EndsWith("%"))
                    {
                        var ratio = text.Substring(0, text.Length - 1).ToDouble() / 100.0;
                        var filePath = Path.Combine(currentDirectory, currentFile);
                        var fileInfo = _sourceFiles.First(x => x.FilePath == filePath);
                        if (fileInfo != null)
                        {
                            fileInfo.CopiedSize = (long)(fileInfo.TotalSize * ratio);
                            Interlocked.Add(ref TotalCopiedSize, fileInfo.CopiedSize - prevCopiedSize);
                            OnProgressChanged?.Invoke(TotalCopiedSize, TotalSize, TotalProgress);
                            prevCopiedSize = fileInfo.CopiedSize;
                        }
                    }
                    // text.Dump();
                };
                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
            }

            OnComplete?.Invoke();
        }
    }

    public class DirectoryCopy
    {
        private readonly string _sourcePath;
        private readonly string _targetPath;

        private readonly List<FileInfo> _sourceFiles;

        public long TotalSize => _sourceFiles.Sum(x => x.Length);
        public long TotalCopiedSize = 0;
        public double TotalProgress => ((double)TotalCopiedSize) / TotalSize;

        public event ProgressChangeDelegate OnProgressChanged;
        public event CompleteDelegate OnComplete;

        public DirectoryCopy(string sourcePath, string targetPath, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories, Func<string, bool> filter = null)
        {
            if (filter == null)
            {
                filter = filePath => true;
            }

            _sourcePath = sourcePath;
            _targetPath = targetPath;
            _sourceFiles = Directory.GetFiles(sourcePath, searchPattern, searchOption)
                .Where(filter)
                .Select(x => new FileInfo(x))
                .ToList();
        }

        public async Task CopyAsync()
        {
            const int bufferSize = 1024 * 1024;
            var buffer = new byte[bufferSize];

            foreach (var fileInfo in _sourceFiles)
            {
                var sourceFilePath = fileInfo.FullName;
                var referenceSourcePath = sourceFilePath.Replace(_sourcePath, "");
                var checkChars = new[] { @"\", "/" };
                if (checkChars.Any(x => referenceSourcePath.StartsWith(x)))
                {
                    referenceSourcePath = referenceSourcePath.Substring(1);
                }
                var targetFilePath = Path.Combine(_targetPath, referenceSourcePath);

                var prevCopiedSize = 0L;
                var fileCopy = new FileCopy(sourceFilePath, targetFilePath);
                fileCopy.OnProgressChanged += (copiedSize, totalSize, progress) =>
                {
                    Interlocked.Add(ref TotalCopiedSize, copiedSize - prevCopiedSize);
                    OnProgressChanged?.Invoke(TotalCopiedSize, TotalSize, TotalProgress);
                    prevCopiedSize = copiedSize;
                };
                await fileCopy.CopyAsync();
            }

            OnComplete?.Invoke();
        }
    }

    public class FileCopy
    {
        private readonly string _sourcePath;
        private readonly string _targetPath;

        public event ProgressChangeDelegate OnProgressChanged;
        public event CompleteDelegate OnComplete;

        public FileCopy(string sourcePath, string targetPath)
        {
            _sourcePath = sourcePath;
            _targetPath = targetPath;
        }

        public async Task CopyAsync()
        {
            const int bufferSize = 1024 * 1024;
            var buffer = new byte[bufferSize];

            var targetParentPath = Directory.GetParent(_targetPath).FullName;
            if (!Directory.Exists(targetParentPath))
            {
                Directory.CreateDirectory(targetParentPath);
            }
            if (File.Exists(_targetPath))
            {
                File.Delete(_targetPath);
            }

            using (var source = new FileStream(_sourcePath, FileMode.Open, FileAccess.Read))
            {
                var totalSize = source.Length;
                using (var target = new FileStream(_targetPath, FileMode.CreateNew, FileAccess.Write))
                {
                    var copiedSize = 0L;
                    var currentBlockSize = 0;

                    while ((currentBlockSize = await source.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        copiedSize += currentBlockSize;
                        var progress = ((double)copiedSize) / totalSize;

                        await target.WriteAsync(buffer, 0, currentBlockSize);

                        OnProgressChanged?.Invoke(copiedSize, totalSize, progress);
                    }
                }
            }

            OnComplete?.Invoke();
        }
    }

}
