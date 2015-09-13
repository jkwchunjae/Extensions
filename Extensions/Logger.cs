using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class Logger
	{
		static DateTime _lastWriteTime = DateTime.Now.AddMinutes(-10);
		static StringBuilder _logBuffer = new StringBuilder();

		public static void Dump(this object value, string name = "")
		{
			if (name == "")
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.WriteLine(name + ": " + value);
			}
		}

		public static void Log(string format, params object[] param)
		{
			if (param.Count() > 0)
				Log(format.With(param));
			else
				Log(format);
		}

		public static void Log<T>(string format, T arg) where T: class
		{
			Log(format.WithVar(arg));
		}

		public static void Log(Exception ex)
		{
			while (true)
			{
				if (ex == null) return;

				Log(ex.Message);
				Log(ex.Source);
				Log(ex.StackTrace);
				ex = ex.InnerException;
			}
		}

		private static void Log(string message)
		{
			try
			{
				string timestamp = DateTime.Now.ToString("[MM-dd HH:mm:ss] ");
				string result = timestamp + message.Replace("\r", "").Split('\n').Select((e, i) => (i > 0 ? "\t\t" : "") + e).StringJoin(Environment.NewLine);

				Console.WriteLine(result);

				string fileName = "{0}.log".With(DateTime.Now.ToString("yyyy_MM_dd"));
				string filePath = "Log/{0}".With(fileName);
#if (DEBUG)
#else
				lock (_logBuffer)
				{
					if (!Directory.Exists("Log"))
						Directory.CreateDirectory("Log");

					_logBuffer.AppendLine(result);
					if (DateTime.Now.Subtract(_lastWriteTime).TotalMinutes > 10.0)
					{
						File.AppendAllText(filePath, _logBuffer.ToString(), Encoding.UTF8);
						_logBuffer.Clear();
						_lastWriteTime = DateTime.Now;
					}
				}
#endif
			}
			catch (Exception ex)
			{
				//Console.WriteLine(ex.Message);
			}
		}
	}
}
