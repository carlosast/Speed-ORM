using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Speed
{

    /// <summary>
    /// Funções úteis de mamipulação de imagem
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class ImageUtil
    {

        #region Win32

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFileInfo(
          string pszPath,
          int dwFileAttributes,
          out    SHFILEINFO psfi,
          uint cbfileInfo,
          SHGFI uFlags);

        [Flags]
        enum SHGFI : int
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocatin = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }

        /// <summary>Maximal Length of unmanaged Windows-Path-strings</summary>
        private const int MAX_PATH = 260;
        /// <summary>Maximal Length of unmanaged Typename</summary>
        private const int MAX_TYPE = 80;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TYPE)]
            public string szTypeName;
        };

        #endregion Win32

        public enum IconSize
        {
            Large = 1,
            Small = 2
        }
        
        /// <summary>
        /// Redimensiona uma imagem para as novas dimensões
        /// </summary>
        /// <param name="b"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Bitmap Stretch(Image b, int newWidth, int newHeight)
        {
            System.Drawing.Bitmap ret = null;
            ret = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(ret))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(b, 0, 0, ret.Width, ret.Height);
            }
            return ret;
        }

        public static void Stretch(string sourceFileName, string targetFileName, int newWidth, int newHeight)
        {
            using (Bitmap b1 = (Bitmap)Image.FromFile(sourceFileName))
            {
                using (var b2 = Stretch(b1, newWidth, newHeight))
                    b2.Save(targetFileName, b1.RawFormat);
            }
        }

        public static void Stretch(string sourceFileName, string targetFileName, double scale)
        {
            using (Bitmap b1 = (Bitmap)Image.FromFile(sourceFileName))
            {
                using (var b2 = Stretch(b1, (int)(b1.Width * scale), (int)(b1.Height * scale)))
                    b2.Save(targetFileName, b1.RawFormat);
            }
        }

        public static void StretchProportional(string sourceFileName, string targetFileName, Size maxSize)
        {
            using (Bitmap b1 = (Bitmap)Image.FromFile(sourceFileName))
            {
                if (b1.Width > b1.Height)
                {
                    //landscape 
                    maxSize.Height = b1.Height / (b1.Width / maxSize.Width);
                }
                else if (b1.Height > b1.Width)
                {
                    //portrait 
                    maxSize.Width = b1.Width / (b1.Height / maxSize.Height);
                }
                else if (b1.Width == b1.Height)
                {
                    //square 
                    if (maxSize.Width > maxSize.Height)//get the shortest side 
                        maxSize.Width = maxSize.Height;
                    else
                        maxSize.Height = maxSize.Width;
                }

                using (var b2 = Stretch(b1, maxSize.Width, maxSize.Height))
                    b2.Save(targetFileName, b1.RawFormat);
            }
        }

        /*
            public static void ResizePicture(string originalpath, string newpath, Size newsize)
            {
                using (Bitmap oldbmp = Bitmap.FromFile(originalpath) as Bitmap)
                {
                    if (oldbmp.Width > oldbmp.Height)
                    {
                        //landscape 
                        newsize.Height = oldbmp.Height / (oldbmp.Width / newsize.Width);
                    }
                    else if (oldbmp.Height > oldbmp.Width)
                    {
                        //portrait 
                        newsize.Width = oldbmp.Width / (oldbmp.Height / newsize.Height);
                    }
                    else if (oldbmp.Width == oldbmp.Height)
                    {
                        //square 
                        if (newsize.Width > newsize.Height)//get the shortest side 
                            newsize.Width = newsize.Height;
                        else
                            newsize.Height = newsize.Width;
                    }
                    using (Bitmap newbmp = new Bitmap(newsize.Width, newsize.Height))
                    {
                        using (Graphics newgraphics = Graphics.FromImage(newbmp))
                        {
                            newgraphics.Clear(
                            Color.FromArgb(-1));
                            newgraphics.DrawImage(oldbmp, 0, 0, newsize.Width, newsize.Height);
                            newgraphics.Save();
                            newbmp.Save(newpath, System.Drawing.Imaging.
                            ImageFormat.Jpeg);
                        }
                    }
                }
                //end using oldbmp 
            }//end method
            */
        public static Bitmap Clip(Image b, int newWidth, int newHeight)
        {
            System.Drawing.Bitmap ret = null;
            ret = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(ret))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImageUnscaledAndClipped(b, new Rectangle(0, 0, b.Width, b.Height));
                // g.DrawImageUnscaled(b, new Rectangle(0, 0, newWidth, newHeight));
                // g.DrawImageUnscaled(b, new Rectangle(0, 0, ret.Width, ret.Height));
                // g.DrawImageUnscaledAndClipped(b, new Rectangle(0, 0, ret.Width, ret.Height));
            }
            return ret;
        }

        public static Image Tile(Image b, int tileWidth, int tileHeight)
        {
            Bitmap ret = new Bitmap(tileWidth, tileHeight, b.PixelFormat);

            using (Graphics g = Graphics.FromImage(ret))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                for (int left = 0; left < tileWidth; left += b.Width)
                {
                    for (int top = 0; top < tileHeight; top += b.Height)
                    {
                        g.DrawImage(b, left, top);
                    }
                }
            }

            return ret;
        }

        public static Image Tile(Image b, int newWidth, int newHeight, int tileWidth, int tileHeight)
        {
            using (Image b2 = (b.Width == newWidth && b.Height == newHeight) ? b : Stretch(b, newWidth, newHeight))
                return Tile(b2, tileWidth, tileHeight);
        }

        public static byte[] ToByteArray(Bitmap b, ImageFormat format)
        {
            using (MemoryStream m = new MemoryStream())
            {
                b.Save(m, format);
                return m.ToArray();
            }
        }

        public static Bitmap FromByteArray(byte[] buffer)
        {
            if (buffer == null)
                return null;
            using (MemoryStream ms = new MemoryStream(buffer))
                return new Bitmap(ms);
        }

        public static Image Convert(Image b, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                b.Save(ms, format);
                return Bitmap.FromStream(ms);
            }
        }

        public static Icon ConvertToIcon(Bitmap bitmap)
        {
            return Icon.FromHandle(bitmap.GetHicon());
        }

        public static Size GetImageSize(string fileName)
        {
            System.Drawing.Size size;
            using (var img = System.Drawing.Image.FromFile(fileName))
                size = img.Size;
            return size;
        }

        public static Icon GetIcon(string filename, IconSize size)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            if (size == IconSize.Small)
                SHGetFileInfo(filename, 0, out shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI.Icon | SHGFI.SmallIcon);
            else
                SHGetFileInfo(filename, 0, out shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI.Icon | SHGFI.LargeIcon);

            // The icon is returned in the hIcon member of the shinfo struct.
            return Icon.FromHandle(shinfo.hIcon);
        }

        /// <summary>
        /// Carrega uma imagem de um arquuivo. Diferente de Image.FromFile que mantém o arquivo
        /// bloqueado, LoadImage não bloqueia o arquivo
        /// </summary>
        /// <param name="fileFullName"></param>
        /// <returns></returns>
        public static Image LoadImage(string fileFullName)
        {
            Stream fileStream = File.OpenRead(fileFullName);
            Image image = Image.FromStream(fileStream);

            // PropertyItems seem to get lost when fileStream is closed to quickly (?); perhaps 
            // this is the reason Microsoft didn't want to close it in the first place. 
            PropertyItem[] items = image.PropertyItems;

            fileStream.Close();

            foreach (PropertyItem item in items)
            {
                image.SetPropertyItem(item);
            }

            return image;
        } 

    }

}
