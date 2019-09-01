using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace Speed.IO
{

    public static class FileTools
    {

        /// <summary>
        /// Procuras no diretório, obdecendo os filtros, e contas as extenssões de todos os arquivos
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="searchPattern">searchPattern do nome do diretório</param>
        /// <param name="searchOption">Recursivo ou não</param>
        /// <returns></returns>
        public static List<string> FileFindExtensions(string path, string searchPattern, SearchOption searchOption)
        {
            List<string> ret = new List<string>();
            foreach (var file in Directory.GetFiles(path, searchPattern, searchOption))
            {
                string ext = Path.GetExtension(file).ToLower();
                if (!ret.Contains(ext))
                    ret.Add(ext);
            }
            ret.Sort((v1, v2) => v1.CompareTo(v2));
            return ret;
        }

        /// <summary>
        /// Copia todos os arquivos, com as extensions fornecidas, localizado no diretório "dirSource" para
        /// o diretório dirTarget
        /// </summary>
        /// <param name="dirSource"></param>
        /// <param name="dirTarget"></param>
        /// <param name="searchOption"></param>
        /// <param name="extensions">Extensions. Ex: ".png", ".gif"</param>
        /// <returns></returns>
        public static List<string> FileCopyExtensions(string dirSource, string dirTarget, SearchOption searchOption, params string[] extensions)
        {
            List<string> exts = new List<string>(extensions);
            List<string> dirCreate = new List<string>();
            List<string> copys = new List<string>();
            foreach (var file in Directory.GetFiles(dirSource, "*.*", searchOption))
            {
                string ext = Path.GetExtension(file).ToLower();
                if (exts.Contains(ext))
                {
                    string dir = Path.Combine(dirTarget, "Files_" + ext);
                    if (!dirCreate.Contains(dir))
                    {
                        dirCreate.Add(ext);
                        Directory.CreateDirectory(dir);
                    }
                    string filename = Path.Combine(dir, Path.GetFileName(file));
                    try
                    {
                        File.Copy(file, filename, true);
                        Console.WriteLine(filename);
                        copys.Add(filename);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(file + " - " + ex.Message);
                    }
                }
            }
            copys.Sort((v1, v2) => v1.CompareTo(v2));
            return copys;
        }

        /*
        /// <summary>
        /// Copia todos os arquivos de imagens, com as extensions fornecidas, localizado no diretório "dirSourced" para
        /// o diretório dirTarget. Separa por tamanho das imagens
        /// </summary>
        /// <param name="dirSource"></param>
        /// <param name="dirTarget"></param>
        /// <param name="searchOption"></param>
        /// <param name="extensions">Extensions. Ex: ".png", ".gif"</param>
        /// <returns></returns>
        public static List<string> FileCopyExtensionsImages(string dirSource, string dirTarget, SearchOption searchOption, params string[] extensions)
        {
            List<string> exts = new List<string>(extensions);
            List<string> dirCreate = new List<string>();
            List<string> copys = new List<string>();
            foreach (var file in Directory.GetFiles(dirSource, "*.*", searchOption))
            {
                string ext = Path.GetExtension(file).ToLower();
                if (exts.Contains(ext))
                {
                    Size size = GetImageSize(file);
                    if (size.IsEmpty)
                    {
                        string dir = Path.Combine(dirTarget, "Images_" + ext.Replace(".", ""));
                        dir = Path.Combine(dir, size.ToString());
                        if (!dirCreate.Contains(dir))
                        {
                            dirCreate.Add(ext);
                            Directory.CreateDirectory(dir);
                        }
                        string filename = Path.Combine(dir, Path.GetFileName(file));
                        try
                        {
                            File.Copy(file, filename, true);
                            Console.WriteLine(filename);
                            copys.Add(filename);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(file + " - " + ex.Message);
                        }
                    }
                }
            }
            copys.Sort((v1, v2) => v1.CompareTo(v2));
            return copys;
        }
        */

        /// <summary>
        /// Copia todos os arquivos de imagens, com as extensions fornecidas, localizado no diretório "dirSourced" para
        /// o diretório dirTarget. Separa por tamanho das imagens
        /// </summary>
        /// <param name="dirSource"></param>
        /// <param name="dirTarget"></param>
        /// <param name="searchOption"></param>
        /// <param name="extensions">Extensions. Ex: ".png", ".gif"</param>
        /// <returns></returns>
        public static List<string> FileCopyExtensionsImages(string dirSource, string dirTarget, SearchOption searchOption, int multiple, params string[] extensions)
        {
            List<string> exts = new List<string>(extensions);
            //string search = "";
            //foreach (var e in extensions)
            //search += "*" + e + ";";

            List<string> dirCreate = new List<string>();
            List<string> copys = new List<string>();
            using (Speed.IO.FileSystemEnumerator f = new Speed.IO.FileSystemEnumerator(dirSource, "*.*", true, false))
            {
                foreach (var file in f.Matches())
                {
                    try
                    {
                        string ext = file.Extension.ToLower();
                        if (exts.Contains(ext))
                        {
                            Size size = GetImageSize(file.FullName);
                            if (!size.IsEmpty)
                            {
                                string dir = Path.Combine(dirTarget, "Images_" + ext.Replace(".", ""));
                                // dir = Path.Combine(dir, Math2.Multiple(size.Width, multiple) + "x" + Math2.MultipleGt(size.Height, multiple));
                                dir = Path.Combine(dir, (size.Width + "x" + size.Height));
                                if (!dirCreate.Contains(dir))
                                {
                                    dirCreate.Add(dir);
                                    Directory.CreateDirectory(dir);
                                }
                                string filename = Path.Combine(dir, file.Name);
                                try
                                {
                                    if (File.Exists(filename))
                                        filename = GetUniqueFileName(file.FullName);

                                    ((FileInfo)file).CopyTo(filename, true);
                                    Console.WriteLine(file.FullName);
                                    copys.Add(filename);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(file + " - " + ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            copys.Sort((v1, v2) => v1.CompareTo(v2));
            return copys;
        }

        /// <summary>
        /// Retorna o tamanho de um arquivo de imagem
        /// Se não for um arquivo de imagem retorna Size.Empty
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Size GetImageSize(string filename)
        {
            try
            {
                using (Image b = Bitmap.FromFile(filename))
                    return new Size(b.Width, b.Height);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return Size.Empty;
        }

        public static string GetUniqueFileName(string filename)
        {
            string f = filename;
            string dir = Path.GetDirectoryName(filename);
            string name = Path.GetFileNameWithoutExtension(filename) + " - Copy ";
            string ext = Path.GetExtension(filename);

            int count = 1;
            while (File.Exists(f))
            {
                f = Path.Combine(dir, name + count + ext);
            }
            return f;
        }

        public static Exception FileDeleteSafe(string filename)
        {
            Exception ret = null;
            try
            {
                File.Delete(filename);
                ret = null;
            }
            catch (Exception ex)
            {
                try
                {
                    FileInfo f = new FileInfo(filename);
                    if (f.Exists)
                    {
                        f.IsReadOnly = false;
                        f.Attributes = FileAttributes.Normal;
                        f.Delete();
                        ret = null;
                    }
                    else
                        ret = ex;
                }
                catch (Exception ex2)
                {
                    ret = ex2;
                }
            }
            return ret;
        }

        /// <summary>
        /// Apaga vários arquivos
        /// </summary>
        /// <param name="filenames"></param>
        public static void FileDeleteSafes(IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
                FileDeleteSafe(filename);
        }


        public static Exception DirectoryDeleteSafe(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    return null;

                foreach (var dir in Directory.GetDirectories(path))
                {
                    DirectoryDeleteSafe(dir);
                }

                foreach (var file in Directory.GetFiles(path))
                {
                    FileDeleteSafe(file);
                }
                try
                {
                    Directory.Delete(path, true);
                }
                catch { }
                return null;
            }
            catch (Exception ex)
            {

                return ex;
            }
        }

        /// <summary>
        /// Retorna true se fileName não é nulo e o arquivo existe 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsValidFile(string fileName)
        {
            if (Conv.HasData(fileName) && File.Exists(fileName))
                return true;
            else
                return false;
        }

        public static int ReadAllBytesFromStream(Stream stream, byte[] buffer)
        {
            // Use this method is used to read all bytes from a stream.
            int offset = 0;
            int totalCount = 0;
            int buffer_size = 32 * 1024;
            while (true)
            {
                int bytesRead = stream.Read(buffer, offset, buffer_size);
                if (bytesRead == 0)
                {
                    break;
                }
                offset += bytesRead;
                totalCount += bytesRead;
            }
            return totalCount;
        }

        public static string ChangePath(string oldDir, string newDir, string filename)
        {
            oldDir = oldDir.Replace("/", @"\");
            if (!oldDir.EndsWith("\\"))
                oldDir += "\\";

            string newFilename = filename.Remove(0, oldDir.Length);
            newFilename = Path.Combine(newDir, newFilename);
            return newFilename;
        }

        public static void MakeWritable(string filename, bool writable)
        {
            FileInfo f = new FileInfo(filename);
            f.IsReadOnly = !writable;
        }

        public static void CopyDirectory(string dirSource, string dirTarget, string filter, SearchOption searchOption)
        {
            string[] files = Directory.GetFiles(dirSource, filter, searchOption);
            foreach (var file in files)
            {
                string filename = ChangePath(dirSource, dirTarget, file);
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                File.Copy(file, filename, true);
            }
        }

        /// <summary>
        /// Seta as datas e atributos de fileNameTarget igual a fileNameSource
        /// </summary>
        /// <param name="fileNameSource"></param>
        /// <param name="fileNameTarget"></param>
        public static void SetDatesAndAttributes(string fileNameSource, string fileNameTarget)
        {
            FileInfo fs = new FileInfo(fileNameSource);
            FileInfo ft = new FileInfo(fileNameTarget);
            if (ft.IsReadOnly)
                ft.IsReadOnly = false;
            if (ft.CreationTime != fs.CreationTime)
                ft.CreationTime = fs.CreationTime;
            if (ft.LastWriteTime != fs.LastWriteTime)
                ft.LastWriteTime = fs.LastWriteTime;
            if (ft.LastAccessTime != fs.LastAccessTime)
                ft.LastAccessTime = fs.LastAccessTime;
            if (ft.Attributes != fs.Attributes)
                ft.Attributes = fs.Attributes;
        }

        public static void SetDates(FileSystemInfo fs, FileSystemInfo ft)
        {
            FileInfo fi = ft as FileInfo;
            if (fi != null && fi.IsReadOnly)
                fi.IsReadOnly = false;
            if (ft.CreationTime != fs.CreationTime)
                ft.CreationTime = fs.CreationTime;
            if (ft.LastWriteTime != fs.LastWriteTime)
                ft.LastWriteTime = fs.LastWriteTime;
            if (ft.LastAccessTime != fs.LastWriteTime)
                ft.LastAccessTime = fs.LastWriteTime;
        }

        public static string GetTempDirectory()
        {
            bool ok = false;
            string dir = null;
            do
            {
                dir = Path.GetTempFileName();
                File.Delete(dir);
                dir = dir.Substring(0, dir.Length - 4);
                ok = !Directory.Exists(dir);
                if (ok)
                    Directory.CreateDirectory(dir);
            } while (ok);
            return dir;
        }

        /// <summary>
        /// Remove todos caracteres inválidos de path e retorna um válido path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToValidPath(string path)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                path = path.Replace(c.ToString(), "");
            foreach (var c in Path.GetInvalidPathChars())
                path = path.Replace(c.ToString(), "");
            return path;
        }

        public static List<string> GetFiles(string dir, string extensions, bool recursive)
        {
            var files = new List<string>();

            if (dir.EndsWith(":"))
                dir += "\\";

            extensions = extensions.Replace(";", ",");
            string[] exts = extensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < exts.Length; i++)
            {
                exts[i] = exts[i].Trim();
                if (exts[i].StartsWith("."))
                    exts[i] = "*" + exts[i];
            }

            Array.Sort(exts);
            exts = exts.ToList().Distinct().ToArray();

            GetFiles(files, dir, exts, recursive);
            files.Sort();

            return files;
        }

        static void GetFiles(List<string> files, string dir, string[] extensions, bool recursive)
        {
            foreach (var ext in extensions)
            {
                try
                {
                    files.AddRange(Directory.GetFiles(dir, ext, SearchOption.TopDirectoryOnly));
                }
                catch { }
            }

            if (recursive)
            {
                try
                {
                    foreach (var _dir in Directory.GetDirectories(dir))
                    {
                        try
                        {
                            GetFiles(files, _dir, extensions, recursive);
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                }
                catch { }
            }
        }


    }

}
