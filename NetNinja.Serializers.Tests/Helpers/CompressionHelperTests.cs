

using NetNinja.Serializers.Helpers;

namespace NetNinja.Serializers.Tests.Helpers
{
    public class CompressionHelperTests
    {
        [Fact]
        public void CompressAndDecompress_ReturnsOriginalData()
        {
            var originalData = new byte[] { 1, 2, 3, 4, 5 };

            var compressedData = CompressionHelper.Compress(originalData);
            var decompressedData = CompressionHelper.Decompress(compressedData);

            Assert.Equal(originalData, decompressedData);
        }

        [Fact]
        public void Compress_EmptyData_ReturnsNonNullOrError()
        {
            var emptyData = Array.Empty<byte>();

            var compressedData = CompressionHelper.Compress(emptyData);

            Assert.NotNull(compressedData);
            Assert.True(compressedData.Length > 0); 
        }

        [Fact]
        public void Decompress_EmptyCompressedData_ReturnsEmpty()
        {
            var compressedData = CompressionHelper.Compress(Array.Empty<byte>());

            var decompressedData = CompressionHelper.Decompress(compressedData);

            Assert.NotNull(decompressedData);
            Assert.Empty(decompressedData);
        }

        [Fact]
        public void Decompress_InvalidData_ThrowsInvalidDataException()
        {
            var invalidData = new byte[] { 0, 1, 2, 3, 4 };

            Assert.Throws<InvalidDataException>(() => CompressionHelper.Decompress(invalidData));
        }

        [Fact]
        public void Decompress_CorruptedCompressedData_ThrowsInvalidDataException()
        {
            var originalData = new byte[] { 1, 2, 3, 4, 5 };
            var compressedData = CompressionHelper.Compress(originalData);

            compressedData[compressedData.Length - 1] = 0xFF; 

            var exception = Assert.Throws<InvalidDataException>(() => CompressionHelper.Decompress(compressedData));
            Assert.Contains("The compressed data is invalid or corrupted", exception.Message);
        }
    }
}