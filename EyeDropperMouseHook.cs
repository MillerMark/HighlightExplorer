using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HighlightExplorer
{
	public class EyeDropperMouseHook
	{
		static Color lastColorAtMouse;
		static Color finalColorAtMouse;

		static TextBox textBox;
		static bool stopOnNextMouseMessage;
		private static IntPtr mouseHook = IntPtr.Zero;
		private static readonly IntPtr WH_MOUSE_LL = new IntPtr(14);
		private static LowLevelMouseProc mouseHookProcedure;
		private static bool mouseHookIsSet;

		public static TextBox TextBox { get => textBox; set => textBox = value; }

		private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
		public static void Set()
		{
			if (mouseHookIsSet)
				return;

			mouseHookProcedure = MouseHookProc;
			using (Process curProcess = Process.GetCurrentProcess())
				using (ProcessModule curModule = curProcess.MainModule)
					mouseHook = SetWindowsHookEx(WH_MOUSE_LL, mouseHookProcedure, GetModuleHandle(curModule.ModuleName), 0);  // This works under Vista and Win7 but doesn't work under XP
			mouseHookIsSet = (mouseHook != IntPtr.Zero);
		}

		public static void Remove()
		{
			if (!mouseHookIsSet)
				return;

			mouseHookIsSet = false;
			if (mouseHook != IntPtr.Zero)
				UnhookWindowsHookEx(mouseHook);
			mouseHook = IntPtr.Zero;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindowDC(IntPtr window);
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern uint GetPixel(IntPtr dc, int x, int y);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int ReleaseDC(IntPtr window, IntPtr dc);

		public static Color GetColorAt(int x, int y)
		{
			IntPtr desk = GetDesktopWindow();
			IntPtr dc = GetWindowDC(desk);
			int a = (int)GetPixel(dc, x, y);
			ReleaseDC(desk, dc);
			return Color.FromArgb(255, (byte)((a >> 0) & 0xff), (byte)((a >> 8) & 0xff), (byte)((a >> 16) & 0xff));
		}

		private static Color GetColorAtMouse()
		{
			System.Drawing.Point absolutePosition = System.Windows.Forms.Cursor.Position;
			return GetColorAt(absolutePosition.X, absolutePosition.Y);
		}

		static IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (stopOnNextMouseMessage)
			{
				if (textBox != null)
					textBox.StopGrabbing();
				Remove();
				stopOnNextMouseMessage = false;
				return CallNextHookEx(mouseHook, nCode, wParam, lParam);
			}
			if (nCode < 0)
			{
				return CallNextHookEx(mouseHook, nCode, wParam, lParam);
			}
			else
			{
				if ((MouseMessages)wParam == MouseMessages.WM_MOUSEMOVE)
				{
					if (textBox != null)
					{
						lastColorAtMouse = GetColorAtMouse();
						textBox.UpdatePreview(lastColorAtMouse);
					}
				}
				else if ((MouseMessages)wParam == MouseMessages.WM_LBUTTONDOWN)
				{
					if (textBox != null)
					{
						finalColorAtMouse = GetColorAtMouse();
						textBox.SetBackgroundColor(Colors.White);
						if (textBox != null)
							textBox.Text = "#" + finalColorAtMouse.R.ToString("X2") + finalColorAtMouse.G.ToString("X2") + finalColorAtMouse.B.ToString("X2");

						stopOnNextMouseMessage = true;
						return new IntPtr(1);
					}
				}

				return CallNextHookEx(mouseHook, nCode, wParam, lParam);
			}

		}

		[DllImport("user32.dll")]
		static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(IntPtr idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		private enum MouseMessages
		{
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_MOUSEMOVE = 0x0200,
			WM_MOUSEWHEEL = 0x020A,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205
		}
	}
}