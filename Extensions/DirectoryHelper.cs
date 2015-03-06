using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public class DirInfo
	{
		protected string _path;
		protected List<DirInfo> _dirList = new List<DirInfo>();
		protected List<FileInfo> _fileList = new List<FileInfo>();
		protected DefaultDictionary<string, DirInfo> _dirMap = new DefaultDictionary<string, DirInfo>();

		public DirInfo(string path)
		{
			_path = path.Replace('\\', '/');
			foreach (var dir in Directory.GetDirectories(_path).OrderBy(e => e))
				_dirList.Add(new DirInfo(dir));
			foreach (var file in Directory.GetFiles(_path).OrderBy(e => e))
				_fileList.Add(new FileInfo(file.Replace('\\', '/')));
			foreach (var dir in _dirList)
				_dirMap.Add(Path.GetFileName(dir._path), dir);
		}

		public DirInfo this [string dirName]
		{
			get
			{
				return _dirList.Where(e => Path.GetFileName(e._path).Contains(dirName)).FirstOrDefault();
			}
		}

		public string FolderName
		{
			get { return Path.GetFileName(_path); }
		}

		public IEnumerable<DirInfo> GetSubDirList()
		{
			return _dirList;
		}

		public IEnumerable<string> GetDirNames()
		{
			return _dirList.Select(e => Path.GetFileName(e._path));
		}

		public IEnumerable<FileInfo> GetFiles()
		{
			return _fileList;
		}
	}

	public static class DirectoryHelper
	{
		public static DirInfo CreateDirInfo(this string path)
		{
			return new DirInfo(path);
		}
	}
}
