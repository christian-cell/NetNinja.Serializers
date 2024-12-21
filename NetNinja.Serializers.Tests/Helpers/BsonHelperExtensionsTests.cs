using NetNinja.Serializers.Helpers;

namespace NetNinja.Serializers.Tests.Helpers
{
    public class BsonHelperExtensionsTests
    {
        [Fact]
        public void ToHexString_BytesToHex_ConvertsCorrectly()
        {
            var bytes = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };

            var hex = bytes.ToHexString();

            Assert.Equal("deadbeef", hex);
        }

        [Fact]
        public void HexToBytes_HexToBytes_ConvertsCorrectly()
        {
            var hex = "deadbeef";

            var bytes = hex.HexToBytes();

            Assert.Equal(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, bytes);
        }

        [Fact]
        public void HexToBytes_And_ToHexString_RoundTrip_ProducesOriginalHex()
        {
            var originalHex = "cafebabe";

            var bytes = originalHex.HexToBytes();
            
            var convertedHex = bytes.ToHexString();

            Assert.Equal(originalHex, convertedHex);
        }

        [Fact]
        public void ToHexString_And_HexToBytes_RoundTrip_ProducesOriginalBytes()
        {
            var originalBytes = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };

            var hex = originalBytes.ToHexString();
            var convertedBytes = hex.HexToBytes();

            Assert.Equal(originalBytes, convertedBytes);
        }

        [Fact]
        public void ToHexString_EmptyBytes_ReturnsEmptyString()
        {
            var bytes = Array.Empty<byte>();

            var hex = bytes.ToHexString();

            Assert.Equal(string.Empty, hex);
        }

        [Fact]
        public void HexToBytes_EmptyHex_ReturnsEmptyBytes()
        {
            var hex = string.Empty;

            var bytes = hex.HexToBytes();

            Assert.Empty(bytes);
        }

        /*[Fact]
        public void HexToBytes_InvalidHex_ThrowsFormatException()
        {
            var invalidHex = "invalid";

            var exception = Assert.Throws<FormatException>(() => invalidHex.HexToBytes());
            Assert.Contains("contains invalid characters", exception.Message);
        }*/

        [Fact]
        public void HexToBytes_OddLengthHex_ThrowsArgumentException()
        {
            var oddLengthHex = "abc";

            var exception = Assert.Throws<ArgumentException>(() => oddLengthHex.HexToBytes());
            Assert.Contains("must have an even length", exception.Message);
        }
    }
}