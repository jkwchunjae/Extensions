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
		public static void Log(string format, params object[] param)
		{
			Log(format.With(param));
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
				if (!Directory.Exists("Log"))
					Directory.CreateDirectory("Log");
				File.AppendAllText(filePath, result + Environment.NewLine, Encoding.UTF8);
			}
			catch (Exception ex)
			{
				//Console.WriteLine(ex.Message);
			}
		}
	}
}
