// postmsg_dotnetframework.implementation2
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace keepalive
{
	static class Program
    {
		public delegate bool EnumWindowDelegate(IntPtr hWnd, IntPtr lparam);

		public delegate bool EnumWindowDelegate2(IntPtr hWnd, IntPtr lparam);

		public struct Rect
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;

			public override string ToString()
			{
				return $"{Left},{Top},{Right},{Bottom}";
			}
		}

		private const int PollIntervalSeconds = 50;

		public static bool BoolMouseLoc = false;

		public static bool BoolNeedToRestore;

		private const int WmSetfocus = 7;

		private const int WmMousemove = 512;

		private const int ALT = 164;

		private const int EXTENDEDKEY = 1;

		private const int KEYUP = 2;

		private static readonly EnumWindowDelegate Callback = EnumWindowProc;

		private static readonly EnumWindowDelegate2 Callback2 = EnumWindowProc2;

		[DllImport("User32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern bool EnumWindows(EnumWindowDelegate wndenumproc, IntPtr lparam);

		[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool EnumChildWindows(IntPtr windowHandle, EnumWindowDelegate2 wndenumproc2, IntPtr lParam);

		[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowText(IntPtr hwnd, byte[] txt, int lng);

		[DllImport("User32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern bool IsWindowVisible(IntPtr hwnd);

		[DllImport("User32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern bool IsIconic(IntPtr hwnd);

		[DllImport("User32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern int GetWindowTextLengthA(IntPtr hwnd);

		[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int ShowWindow(IntPtr hwnd, int lng);

		[DllImport("User32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("User32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern int SetForegroundWindow(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern bool GetClientRect(IntPtr hWnd, ref Rect lpRect);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern void GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern void SetThreadExecutionState(uint esFlags);
		private const uint ES_CONTINUOUS = 0x80000000;
		private const uint ES_DISPLAY_REQUIRED = 0x00000002;
		private const uint ES_SYSTEM_REQUIRED = 0x00000001;

		public static void Main()
		{
			SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
			Console.CancelKeyPress += delegate {
				Console.WriteLine("Exiting the program");
				SetThreadExecutionState(ES_CONTINUOUS);
			};
			while (true)
			{
				BoolNeedToRestore = false;
				BoolMouseLoc = !BoolMouseLoc;
				IntPtr foregroundWindow = GetForegroundWindow();
				EnumWindows(Callback, IntPtr.Zero);
				if (BoolNeedToRestore)
				{
					ShowWindow(foregroundWindow, 5);
					keybd_event(164, 69, 1u, 0);
					keybd_event(164, 69, 3u, 0);
					SetForegroundWindow(foregroundWindow);
				}
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Thread.Sleep(50000);
			}
		}

		private static bool EnumWindowProc(IntPtr hWnd, IntPtr lparam)
		{
			string[] array = new string[5];
			checked
			{
				if (IsWindowVisible(hWnd))
				{
					int windowTextLengthA = GetWindowTextLengthA(hWnd);
					byte[] array2 = new byte[windowTextLengthA * 2 + 1];
					GetWindowText(hWnd, array2, windowTextLengthA + 1);
					string text = "";
					int num = (windowTextLengthA - 1) * 2;
					for (int i = 0; i <= num; i++)
					{
						if (array2[i] != 0)
						{
							text += Conversions.ToString(Strings.Chr(array2[i]));
						}
					}
					if (LikeOperator.LikeString(text.ToLower(), "*virtual workspace*", CompareMethod.Binary))
					{
						Console.WriteLine("Found " + text + ", sending keyboard message...");
						if (IsIconic(hWnd))
						{
							ShowWindow(hWnd, 5);
							ShowWindow(hWnd, 9);
							BoolNeedToRestore = true;
						}
						SendMessage(hWnd, 7, (IntPtr)0, (IntPtr)0);
						Rect lpRect = default(Rect);
						GetClientRect(hWnd, ref lpRect);
						array = Strings.Split(lpRect.ToString(), ",");
						int num2 = (int)Math.Round((double)Conversions.ToInteger(array[2]) / 2.0) * 65536 + (int)Math.Round((double)Conversions.ToInteger(array[3]) / 2.0);
						if (BoolMouseLoc)
						{
							SendMessage(hWnd, 512, (IntPtr)0, (IntPtr)0);
						}
						else
						{
							SendMessage(hWnd, 512, (IntPtr)0, (IntPtr)num2);
						}
						EnumChildWindows(hWnd, Callback2, IntPtr.Zero);
					}
				}
				return true;
			}
		}

		private static bool EnumWindowProc2(IntPtr hWnd, IntPtr lparam)
		{
			string[] array = new string[5];
			int windowTextLengthA = GetWindowTextLengthA(hWnd);
			checked
			{
				byte[] array2 = new byte[windowTextLengthA * 2 + 1];
				GetWindowText(hWnd, array2, windowTextLengthA + 1);
				string text = "";
				int num = (windowTextLengthA - 1) * 2;
				for (int i = 0; i <= num; i++)
				{
					if (array2[i] != 0)
					{
						text += Conversions.ToString(Strings.Chr(array2[i]));
					}
				}
				string windowClass = GetWindowClass((long)hWnd);
				if ((text.Trim().Length != 0) & !MultiContains(text.ToLower(), "disconnected") & !MultiContains(windowClass.ToLower(), "atl:", ".button.", "uicontainerclass", ".systreeview32.", ".scrollbar.", "opwindowclass", "opcontainerclass"))
				{
					Rect lpRect = default(Rect);
					GetClientRect(hWnd, ref lpRect);
					array = Strings.Split(lpRect.ToString(), ",");
					int num2 = (int)Math.Round((double)Conversions.ToInteger(array[2]) / 2.0) * 65536 + (int)Math.Round((double)Conversions.ToInteger(array[3]) / 2.0);
					if (BoolMouseLoc)
					{
						SendMessage(hWnd, 512, (IntPtr)0, (IntPtr)0);
					}
					else
					{
						SendMessage(hWnd, 512, (IntPtr)0, (IntPtr)num2);
					}
				}
				return true;
			}
		}

		public static string GetWindowClass(long hwnd)
		{
			StringBuilder stringBuilder = new StringBuilder("", 256);
			GetClassName((IntPtr)hwnd, stringBuilder, 256);
			return stringBuilder.ToString();
		}

		public static bool MultiContains(this string str, params string[] values)
		{
			return values.Any((string val) => str.Contains(val));
		}
	}
}
