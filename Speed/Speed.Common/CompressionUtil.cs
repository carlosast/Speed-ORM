using System;
using System.Linq;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO.Compression;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Speed
{

    /// <summary>
    /// Utility that peerfomrs compression and decompression
    /// Please see License aggremen t with CSharpZipLib on CodePlex:
    /// http://slsharpziplib.codeplex.com/license
    /// 
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class CompressionUtil
    {

        /// <summary>
        /// Compress string
        /// </summary>
        /// <param name="input">String to compress</param>
        /// <returns>Byte array containing compressed string</returns>
        public static byte[] CompressString(string input)
        {
            return Compress(ConvertStringToBytes(input));
        }

        /// <summary>
        /// Decompress byte array and convert to string
        /// </summary>
        /// <param name="byteInput">Byte to decompress</param>
        /// <returns>String</returns>
        public static string DecompressToString(byte[] byteInput)
        {
            return ConvertBytesToString(Decompress(byteInput));
        }

        /// <summary>
        /// Compress byte array
        /// </summary>
        /// <param name="byteData">Input array</param>
        /// <returns>Compressed byte array</returns>
        public static byte[] Compress(byte[] byteData)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ICSharpCode.SharpZipLib.Zip.Compression.Deflater defl =
                   new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(9, false);
                using (Stream s = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms, defl))
                {
                    s.Write(byteData, 0, byteData.Length);
                    s.Close();
                    byte[] compressedData = (byte[])ms.ToArray();
                    return compressedData;
                }
            }
        }

        /// <summary>
        /// Decompress byte array
        /// </summary>
        /// <param name="byteInput">Array to decompress</param>
        /// <returns>Decompressed array</returns>
        public static byte[] Decompress(byte[] byteInput)
        {
            using (MemoryStream ms = new MemoryStream(byteInput, 0, byteInput.Length))
            {
                byte[] bytResult = null;
                using (Stream s2 = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(ms))
                {
                    bytResult = ReadFullStream(s2);
                    s2.Close();
                    return bytResult;
                }
            }
        }

        /// <summary>
        /// Read entire stream into a byte array
        /// </summary>
        /// <param name="stream">Stream to read</param>
        /// <returns>Byte array</returns>
        public static byte[] ReadFullStream(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        /// <summary>
        /// Convert a string into an array of bytes
        /// </summary>
        /// <param name="input">String to convert</param>
        /// <returns>Byte atray that repsesents the string</returns>
        public static byte[] ConvertStringToBytes(string input)
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(input);
                writer.Flush();
            }
            return stream.ToArray();
        }

        /// <summary>
        /// Convert abyte array into a string
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <returns>String constructed from bytes</returns>
        public static string ConvertBytesToString(byte[] bytes)
        {
            string output = String.Empty;
            MemoryStream stream = new MemoryStream(bytes);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                output = reader.ReadToEnd();
            }
            return output;
        }

        public static void UnZip(string fileName, string dirTarget)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(fileName))
            {
                zip.ExtractAll(dirTarget, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
            }

            //using (ZipInputStream s = new ZipInputStream(File.OpenRead(fileName)))
            //{
            //    ZipEntry entry;
            //    while ((entry = s.GetNextEntry()) != null)
            //    {

            //        string fName = Path.GetFileName(entry.Name);
            //        if (entry.Name == "Documents/1/Metadata/Page1_Thumbnail.JPG")
            //            entry.Name.ToString();
            //        //Directory.CreateDirectory(dirTarget);
            //        if (fName != String.Empty)
            //        {
            //            fName = Path.Combine(dirTarget, entry.Name);
            //            FileInfo fi = new FileInfo(fName);
            //            if (fi.Exists)
            //            {
            //                fi.IsReadOnly = false;
            //                File.Delete(fName);
            //            }
            //            Directory.CreateDirectory(Path.GetDirectoryName(fName));
            //            FileStream streamWriter = File.Create(fName);
            //            int size = 2048;
            //            byte[] data = new byte[2048];
            //            while (true)
            //            {
            //                size = s.Read(data, 0, data.Length);
            //                if (size > 0)
            //                {
            //                    streamWriter.Write(data, 0, size);
            //                }
            //                else
            //                {
            //                    break;
            //                }
            //            }
            //            streamWriter.Close();
            //            fi.CreationTime = fi.LastWriteTime = entry.DateTime;
            //        }
            //    }
            //    s.Close();
            //}
        }

        public static void ZipDirectory(string zipFileName, string dirToZip)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(zipFileName))
            {
                zip.AddDirectory(dirToZip);
                zip.Save();
            }
        }

        public static void ZipFile(string FileToZip, string ZipedFile, int CompressionLevel, int BlockSize)
        {
            throw new Exception("Não está testada.");
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborderd");
            }

            System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            ZipEntry ZipEntry = new ZipEntry(ZipedFile);
            ZipStream.PutNextEntry(ZipEntry);
            ZipStream.SetLevel(CompressionLevel);
            byte[] buffer = new byte[BlockSize];
            System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
            ZipStream.Write(buffer, 0, size);
            try
            {
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            ZipStream.Finish();
            ZipStream.Close();
            StreamToZip.Close();
        }

        public static void ZipFileMain(string[] args)
        {
            string[] filenames = Directory.GetFiles(args[0]);

            Crc32 crc = new Crc32();
            ZipOutputStream s = new ZipOutputStream(File.Create(args[1]));

            s.SetLevel(6); // 0 - store only to 9 - means best compression 

            foreach (string file in filenames)
            {
                FileStream fs = File.OpenRead(file);

                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(file);

                entry.DateTime = DateTime.Now;
                // set Size and the crc, because the information 
                // about the size and crc should be stored in the header 
                // if it is not set it is automatically written in the footer. 
                // (in this case size == crc == -1 in the header) 
                // Some ZIP programs have problems with zip files that don't store 
                // the size and crc in the header. 
                entry.Size = fs.Length;
                fs.Close();

                crc.Reset();
                crc.Update(buffer);

                entry.Crc = crc.Value;

                s.PutNextEntry(entry);

                s.Write(buffer, 0, buffer.Length);

            }

            s.Finish();
            s.Close();
        }

        /// <summary>
        /// Comprime sourcePath em targetPath usando o algoritmo gzip
        /// </summary>
        /// <param name="sourcePath">Arquivo descompactado</param>
        /// <param name="targetPath">Arquivo compactado</param>
        public static void GzipCompress(string sourcePath, string targetPath)
        {
            using (var target = new FileStream(targetPath, FileMode.Create, FileAccess.Write))
            {
                using (var gzip = new System.IO.Compression.GZipStream(target, CompressionMode.Compress))
                {
                    var buffer = File.ReadAllBytes(sourcePath);
                    gzip.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// OK
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GzipCompress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream ds = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    ds.Write(data, 0, data.Length);
                    ds.Close();
                    ms.Position = 0;
                    byte[] compressed = new byte[ms.Length + 4]; // an extra 4 bytes for original data length
                    Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, compressed, 0, 4);
                    ms.Read(compressed, 4, compressed.Length - 4);
                    return compressed;
                }
            }
        }

        /// <summary>
        /// OK
        /// </summary>
        /// <param name="compressedData"></param>
        /// <returns></returns>
        public static byte[] GzipDecompress(byte[] compressedData)
        {
            int dataLength = BitConverter.ToInt32(compressedData, 0);
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(compressedData, 4, compressedData.Length - 4);
                byte[] data = new byte[dataLength];
                ms.Position = 0;
                using (GZipStream ds = new GZipStream(ms, CompressionMode.Decompress))
                {
                    ds.Read(data, 0, data.Length);
                    return data;
                }
            }
        }

        private static void Pump(Stream input, Stream output)
        {
            byte[] bytes = new byte[4096];
            int n;
            while ((n = input.Read(bytes, 0, bytes.Length)) != 0)
            {
                output.Write(bytes, 0, n);
            }
        }

        public static int ReadAllBytesFromStream(Stream stream, byte[] buffer)
        {
            // Use this method is used to read all bytes from a stream.
            int offset = 0;
            int totalCount = 0;
            int buffer_size = 1024 * 1024;
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

        /// <summary>
        /// Descomprime sourcePath (no formato gzip) em targetPath
        /// </summary>
        /// <param name="sourcePath">Arquivo compactado</param>
        /// <param name="targetPath">Arquivo descompactado</param>
        public static void GzipDecompress(string sourcePath, string targetPath)
        {
            using (FileStream fs = File.OpenRead(sourcePath))
            {
                using (GZipStream gdzip = new GZipStream(fs, CompressionMode.Decompress, true))
                {
                    //Recupera o tamanho do arquivo original antes de zippar.
                    //Ps: Seguindo o que esta descrito no “RFC 1952 GZIP file format specification version 4.3″
                    //http://www.gzip.org/zlib/rfc-gzip.html
                    //Esta informação esta no final do arquivo nos ultimos 4 bytes.

                    //Vai para a posição onde esta o tamanho do arquivo original.
                    fs.Position = (int)fs.Length - 4;

                    //Recupera o tamanho do arquivo original
                    byte[] bufferTamanhoDestino = new byte[4];
                    fs.Read(bufferTamanhoDestino, 0, 4);
                    int tamanhoBufferSaida = BitConverter.ToInt32(bufferTamanhoDestino, 0);

                    //Inicializa o buffer que vai receber o arquivo descompactando
                    byte[] bufferSaida = new byte[tamanhoBufferSaida + 100];

                    //Volta para o início do arquivo
                    fs.Position = 0;

                    int readOffset = 0;
                    int totalBytes = 0;

                    //Percorre o stream e joga para o buffer de saída.
                    int bytesRead;
                    while ((bytesRead = gdzip.Read(bufferSaida, readOffset, 100)) != 0)
                    {
                        readOffset += bytesRead;
                        totalBytes += bytesRead;
                    }
                    //Escreve o buffer para o arquivo de saída.
                    File.WriteAllBytes(targetPath, bufferSaida);
                }
            }
        }

        public static List<Ionic.Zip.ZipEntry> GetEntries(string fileName)
        {
            using (var zip = new Ionic.Zip.ZipFile(fileName))
                return zip.Entries.ToList();
        }

        public static bool ChangeDateIfIsValid(string fileName)
        {
            var entries = GetEntries(fileName);
            if (entries.Count == 1)
            {
                var entry = entries[0];
                if (!entry.IsDirectory && Path.GetFileNameWithoutExtension(entries[0].FileName).ToLower() == Path.GetFileNameWithoutExtension(fileName).ToLower())
                {
                    if (fileName.Contains("C1.Silverlight.Data"))
                        fileName.ToString();
                    File.SetCreationTime(fileName, entry.LastModified);
                    File.SetLastWriteTime(fileName, entry.LastModified);
                    File.SetLastAccessTime(fileName, entry.LastModified);
                    //f.CreationTime = entry.CreationTime;
                    return true;
                }
            }
            return false;
        }

        public static bool UnzipDir(string qZipSource, string qUnZipTo)
        {
            int tThereWereErrors = 0;
            //Clean arguments: 
            if (qUnZipTo == null) { qUnZipTo = string.Empty; }
            if (qUnZipTo == string.Empty) { qUnZipTo = qZipSource; }

            //If the Zip file does not exist, exit now: 
            if (!System.IO.File.Exists(qZipSource))
            {
                throw new System.ArgumentException("qZipSource", "ZipFile not found:" + qZipSource);
            }


            //Extract directory to upload to: 
            //...Clean up Slashes to Win format: 
            string tOutputBaseDir = qUnZipTo.Replace("/", "\\");

            //Ready to roll: 
            ZipInputStream s = null;
            try
            {
                s = new ZipInputStream(File.OpenRead(qZipSource));

                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null)
                {

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);


                    //Create directory if doesn't exist: 
                    string tOutDir = Path.Combine(tOutputBaseDir, directoryName);
                    if (!Directory.Exists(tOutDir))
                        Directory.CreateDirectory(tOutDir);

                    //Make File if valid name: 
                    if (fileName != String.Empty)
                    {
                        byte[] data = new byte[s.Length];
                        s.Read(data, 0, data.Length);
                        File.WriteAllBytes(Path.Combine(tOutDir, fileName),
                            data);
                        //FileStream streamWriter = null;
                        //try
                        //{
                        //    streamWriter = File.Create(Path.Combine(tOutDir, fileName));
                        //    int size = 2048;
                        //    byte[] data = new byte[2048];
                        //    while (true)
                        //    {
                        //        size = s.Read(data, 0, data.Length);
                        //        if (size > 0)
                        //        {
                        //            streamWriter.Write(data, 0, size);
                        //        }
                        //        else
                        //        {
                        //            break;
                        //        }
                        //    }
                        //}
                        //finally
                        //{
                        //    tThereWereErrors += 1;
                        //    if (streamWriter != null) { streamWriter.Close(); }
                        //}
                    }
                }
                s.Close();
            }
            catch (System.Exception E)
            {
                tThereWereErrors += 1;
                throw E;
            }
            finally
            {
                if (s != null) { s.Close(); }
            }
            return false;
        }

    }
}
