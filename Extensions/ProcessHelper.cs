using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Extensions
{
	public class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
		[DllImport("user32.dll")]
		public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);
	}

	public enum MouseEventFlags
	{
		LEFTDOWN = 0x00000002,
		LEFTUP = 0x00000004,
		MIDDLEDOWN = 0x00000020,
		MIDDLEUP = 0x00000040,
		MOVE = 0x00000001,
		ABSOLUTE = 0x00008000,
		RIGHTDOWN = 0x00000008,
		RIGHTUP = 0x00000010
	}

	public static class ProcessHelper
	{
		[DllImport("user32.dll")]
		static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

		[DllImport("User32.Dll")]
		public static extern long SetCursorPos(int x, int y);

		public static Bitmap CaptureWindow(this Process process)
		{
			var rect = new User32.RECT();
			User32.GetWindowRect(process.MainWindowHandle, ref rect);

			int width = rect.right - rect.left;
			int height = rect.bottom - rect.top;

			var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(bmp);
			graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
			return bmp;
		}

		public static IEnumerable<Process> GetProcessesByWindowTitle(string mainWindowTitle)
		{
			return Process.GetProcesses()
				.Where(x => x.MainWindowTitle.Contains(mainWindowTitle));
		}

		public static void MouseMove(this Process process, int x, int y)
		{
			var hWnd = process.MainWindowHandle;
			var rect = new User32.RECT();
			User32.GetWindowRect(hWnd, ref rect);
			var moveX = rect.left + x;
			var moveY = rect.top + y;

			SetCursorPos(moveX, moveY);
		}

		public static void MouseLeftClick(this Process process, int x, int y)
		{
			process.MouseMove(x, y);
			mouse_event((uint)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
			mouse_event((uint)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
		}

		public static void MouseRightClick(this Process process, int x, int y)
		{
			process.MouseMove(x, y);
			mouse_event((uint)(MouseEventFlags.RIGHTDOWN), 0, 0, 0, 0);
			mouse_event((uint)(MouseEventFlags.RIGHTUP), 0, 0, 0, 0);
		}
	}
}
