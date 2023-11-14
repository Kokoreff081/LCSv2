using System.IO.Compression;
using System.Text;

namespace LCSVersionControl;

public static class ZipHelper
    {
        /// <summary>
        /// Архивирует входную строку в byte[]
        /// для архивации в памяти
        /// </summary>
        /// <param name="textToZip">строка для архивации</param>
        /// <returns>архивировнный массив byte[]</returns>
        public static byte[] ZipStringToByteArray(string textToZip)
        {
            using var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var demoFile = zipArchive.CreateEntry("zipped_asset");

                using (var entryStream = demoFile.Open())
                {
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(textToZip);
                    }
                }
            }

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Архивирует входной byte[] в byte[]
        /// для архивации в памяти
        /// </summary>
        /// <param name="inputBuffer">массив байт для сжатия</param>
        /// <returns>архивировнный массив byte[]</returns>
        public static byte[] ZipByteArrayToByteArray(byte[] inputBuffer)
        {
            using var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var demoFile = zipArchive.CreateEntry("zipped_asset");

                using (var entryStream = demoFile.Open())
                {
                    entryStream.Write(inputBuffer, 0, inputBuffer.Length);
                }
            }

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Распакует входной архивированный byte[] в строку
        /// </summary>
        /// <param name="zippedBuffer">входной массив byte[]</param>
        /// <param name="encoding"></param>
        /// <returns>распакованная строка</returns>
        // public static string UnzipByteArrayToString(byte[] zippedBuffer, Encoding encoding = null)
        // {
        //     encoding ??= Encoding.Default;
        //
        //     var unzippedByteArray = UnzipByteArrayToByteArray(zippedBuffer);
        //
        //     return unzippedByteArray == null ? null : encoding.GetString(unzippedByteArray);
        // }

        /// <summary>
        /// Распакует входной архивированный byte[] в byte[]
        /// </summary>
        /// <param name="zippedBuffer">входной массив byte[]</param>
        /// <returns>распакованная byte[]</returns>
        public static byte[] UnzipByteArrayToByteArray(byte[] zippedBuffer)
        {
            using var zippedStream = new MemoryStream(zippedBuffer);
            using var archive = new ZipArchive(zippedStream);
            var entry = archive.Entries.FirstOrDefault();

            if (entry == null) return null;

            using var unzippedEntryStream = entry.Open();
            using var ms = new MemoryStream();
            unzippedEntryStream.CopyTo(ms);
            return ms.ToArray();
        }
        
        public static string UnzipFileToString(string filename)
        {
            if (!File.Exists(filename))
            {
                //LogManager.GetCurrentClassLogger().Error($"File not found: {filename}");
                return string.Empty;
            }
            
            //var bytes = File.ReadAllBytes(filename);
            using var zippedStream = File.OpenRead(filename);
            using var archive = new ZipArchive(zippedStream);
            var entry = archive.Entries.FirstOrDefault();
        
            if (entry != null)
            {
                using var unzippedEntryStream = entry.Open();
                //using var ms = new MemoryStream();
                //unzippedEntryStream.CopyTo(ms);
                //ms.Position = 0;
                using StreamReader reader = new StreamReader(unzippedEntryStream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        
            return string.Empty;
        }
        
        public static Stream UnzipFileToStream(string filename)
        {
            if (!File.Exists(filename))
            {
                //LogManager.GetCurrentClassLogger().Error($"File not found: {filename}");
                return null;
            }
            
            //using var zippedStream = File.OpenRead(filename);
            using var archive = ZipFile.OpenRead(filename);//new ZipArchive(zippedStream, ZipArchiveMode.Read);
            var entry = archive.Entries.FirstOrDefault();
        
            if (entry != null)
            {
                using var unzippedEntryStream = entry.Open();
                 //   unzippedEntryStream.Flush();
                MemoryStream ms = new MemoryStream((int)entry.Length);
                unzippedEntryStream.CopyTo(ms);
                unzippedEntryStream.Flush();
                ms.Position = 0;
                return ms;
            }
        
            return null;
        }
        
        
        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public static Stream StringToStream(string src)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(src);
            return new MemoryStream(byteArray);
        }
        
        public static byte[] Compress(byte[] data)
        {
            using var compressedStream = new MemoryStream();
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Compress);
            zipStream.Write(data, 0, data.Length);
            zipStream.Close();
            return compressedStream.ToArray();
        }
        
        public static byte[] Decompress(byte[] data)
        {
            using var compressedStream = new MemoryStream(data);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var resultStream = new MemoryStream();
            zipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
    }