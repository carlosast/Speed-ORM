using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Speed
{
    /// <summary>
    /// Structure that maps to WIN32_FIND_DATA
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class FindData
    {
        public int fileAttributes;
        public int creationTime_lowDateTime;
        public int creationTime_highDateTime;
        public int lastAccessTime_lowDateTime;
        public int lastAccessTime_highDateTime;
        public int lastWriteTime_lowDateTime;
        public int lastWriteTime_highDateTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public String fileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public String alternateFileName;
    }

    /// <summary>
    /// Wrapper for P/Invoke methods used by FileSystemEnumerator
    /// </summary>
    [SecurityPermissionAttribute(SecurityAction.Demand, UnmanagedCode = true)]
    public static class SafeNativeMethods
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindFirstFile(String fileName, [In, Out] FindData findFileData);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindNextFile(IntPtr hFindFile, [In, Out] FindData lpFindFileData);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindClose(IntPtr hFindFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CopyFile(string strSourcePath, string strTargetPath, bool bOverwrite);
    }

    /// <summary>
    /// SafeHandle class for holding find handles
    /// </summary>
    public class SafeFindHandle : Microsoft.Win32.SafeHandles.SafeHandleMinusOneIsInvalid
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="h">Find handle returned by FindFirstFile</param>
        public SafeFindHandle(IntPtr h)
            : base(true)
        {
            handle = h;
        }

        /// <summary>
        /// Release the find handle
        /// </summary>
        /// <returns>true if the handle was released</returns>
        protected override bool ReleaseHandle()
        {
            return SafeNativeMethods.FindClose(handle);
        }
    }

}
