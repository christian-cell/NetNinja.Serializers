using System.IO.Compression;

namespace NetNinja.Serializers.Helpers
{
    public static class CompressionHelper
    {
        public static byte[] Compress(byte[] data)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
                {
                    gzipStream.Write(data, 0, data.Length);
                }
                return outputStream.ToArray();
            }
        }
        
        public static byte[] Decompress(byte[] compressedData)
        {
            using (var inputStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                try
                {
                    gzipStream.CopyTo(outputStream);
                    return outputStream.ToArray();
                }
                catch (InvalidDataException)
                {
                    throw new InvalidDataException("The compressed data is invalid or corrupted."); // Excepción específica.
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException("An error occurred during decompression.", ex);
                }
            }
        }

        /*public static byte[] Decompress(byte[] compressedData)
        {
            using (var inputStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                gzipStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }*/
    }
};

