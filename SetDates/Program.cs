using Ionic.Zip;
using System;
using System.IO;
using System.Linq;

namespace SetDates
{
    class Program
    {
        static void Main(string[] args)
        {
            string dirPkg = @"D:\Github\Speed\APP\PublishPackages";
            var files = Directory.GetFiles(@"D:\Github\Speed\APP", "*.nupkg", SearchOption.AllDirectories)
                                .Where(p => p.Contains("Release")).ToList();
            files.RemoveAll(p => p.StartsWith(dirPkg));

            DateTime now = DateTime.Now;
            foreach (var file in files)
            {
                Console.WriteLine("Processing: " + file);

                string dir = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".tmp");

                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);

                Unzip(file, dir);

                var files2 = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                foreach (var file2 in files2)
                    File.SetLastWriteTime(file2, now);

                File.Delete(file);

                Zip(file, dir);

                File.Copy(file, Path.Combine(dirPkg, Path.GetFileName(file)), true);

                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);

            }
        }

        private static void Unzip(string zipFileName, string dirExtract)
        {
            using (ZipFile zip = ZipFile.Read(zipFileName))
            {
                zip.ExtractAll(dirExtract);
            }
        }

        private static void Zip(string zipFileName, string dirCompress)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(dirCompress);
                zip.Save(zipFileName);
            }
        }

    }
}
