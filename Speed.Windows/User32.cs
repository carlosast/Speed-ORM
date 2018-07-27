using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Speed.Windows
{
	/// <summary>
	/// Summary description for API.
	/// </summary>
	public class User32
	{

		[DllImport("user32.dll")]
		public static extern int FindWindow(string strclassName, string strWindowName);

		[DllImport("user32.dll")]
		public static extern int SetParent( int hWndChild, int hWndNewParent);

		[DllImport("user32.dll", EntryPoint="SetWindowPos")]
		public static extern bool SetWindowPos(
			int hWnd,               // handle to window
			int hWndInsertAfter,    // placement-order handle
			int X,                  // horizontal position
			int Y,                  // vertical position
			int cx,                 // width
			int cy,                 // height
			uint uFlags             // window-positioning options
			);
		
		[DllImport("user32.dll", EntryPoint="MoveWindow")]
		public static extern bool MoveWindow(
			int hWnd, 
			int X, 
			int Y, 
			int nWidth, 
			int nHeight, 
			bool bRepaint
			);

		[DllImport("user32.dll", EntryPoint = "LoadCursorFromFileW", CharSet = CharSet.Unicode)]
		public static extern IntPtr LoadCursorFromFile(String str);

		public static void SetParent(System.IntPtr hWndChild, System.IntPtr hWndNewParent)
		{
			SetParent(hWndChild.ToInt32(), hWndNewParent.ToInt32());
		}

		public static void SetParent(Form formChild, Form formParent)
		{
			SetParent(formChild.Handle.ToInt32(), formParent.Handle.ToInt32());
		}

		public static void SetParent(IWin32Window hWndChild, IWin32Window hWndNewParent)
		{
			SetParent(hWndChild.Handle.ToInt32(), hWndNewParent.Handle.ToInt32());
		}

		/// <summary>
		/// Draws an icon or cursor.
		/// </summary>
		/// <remarks>Be careful when using this function - I have successfully
		/// blue-screened my system by settting an incorrect (negative)
		/// iStepIfAniCur value.  This may be because it is implemented
		/// in the graphics driver.</remarks>
		[DllImport("user32")]
		public extern static int DrawIconEx(
			IntPtr hDC, 
			int xLeft, 
			int yTop, 
			IntPtr hIcon, 
			int cxWidth, 
			int cyWidth, 
			int istepIfAniCur, 
			IntPtr hbrFlickerFreeDraw, 
			int diFlags);

		[DllImport("user32", CharSet = CharSet.Auto)]
		public extern static IntPtr LoadImage(
			IntPtr hInst, 
			string lpsz, 
			int uType, 
			int cx, 
			int cy, 
			int uFlags);

		[DllImport("user32", CharSet = CharSet.Auto)]
		public extern static IntPtr LoadImage(
			IntPtr hInst, 
			int lpsz, 
			int uType, 
			int cx, 
			int cy, 
			int uFlags);

		[DllImport("user32")]
		public extern static IntPtr CopyImage(
			IntPtr handle, 
			int uType,
			int cxDesired,
			int cyDesired,
			int uFlags);
		
		[DllImport("user32")]
		public extern static int DestroyCursor(
			IntPtr hCursor);

		public const int IMAGE_BITMAP = 0x0;
		public const int IMAGE_ICON = 0x1;
		public const int IMAGE_CURSOR = 0x2;

		public const int LR_DEFAULTCOLOR = 0x0000;
		public const int LR_MONOCHROME = 0x0001;
		public const int LR_COLOR = 0x0002;
		public const int LR_COPYRETURNORG = 0x0004;
		public const int LR_COPYDELETEORG = 0x0008;
		public const int LR_LOADFROMFILE = 0x10;
		public const int LR_LOADTRANSPARENT = 0x20;
		public const int LR_DEFAULTSIZE = 0x0040;
		public const int LR_LOADMAP3DCOLORS = 0x1000;
		public const int LR_CREATEDIBSECTION = 0x2000;
		public const int LR_COPYFROMRESOURCE = 0x4000;

		public const int DI_MASK = 0x1;
		public const int DI_IMAGE = 0x2;
		public const int DI_NORMAL = 0x3;
		public const int DI_COMPAT = 0x4;
		public const int DI_DEFAULTSIZE = 0x8;
	}
}
