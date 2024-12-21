using System.Text.Json;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Factories;

namespace NetNinja.Serializers.Tests.Factory
{
    public class CompareSerializedDataTests
    {
        public class VersionObject : IVersioned
        {
            public string Version { get; set; }
            public string Name { get; set; }
        }

        [Theory]
        [InlineData("{\"Version\":\"1.0\",\"Name\":\"Test\"}", "json", "{\"Version\":\"1.0\",\"Name\":\"Test\"}", "json", true)]
        [InlineData("{\"Version\":\"1.0\",\"Name\":\"Test\"}", "json", "<VersionObject><Version>1.0</Version><Name>Test</Name></VersionObject>", "xml", true)]
        [InlineData("{\"Version\":\"1.0\",\"Name\":\"Test\"}", "json", "{\"Version\":\"2.0\",\"Name\":\"Test\"}", "json", false)]
        [InlineData("{\"Version\":\"1.0\",\"Name\":\"Test\"}", "json", "<VersionObject><Version>2.0</Version><Name>Test</Name></VersionObject>", "xml", false)]
        public void Test_CompareSerializedData(string data1, string format1, string data2, string format2, bool expectedResult)
        {
            var result = SerializerFactory.CompareSerializedData<VersionObject>(data1, format1, data2, format2);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Test_InvalidFormat_ShouldThrowJsonException()
        {
            var invalidData = "Invalid Data";

            Assert.Throws<JsonException>(() =>
            {
                SerializerFactory.CompareSerializedData<VersionObject>(invalidData, "json", invalidData, "json");
            });
        }

        [Fact]
        public void Test_InvalidFormatXml_ShouldThrowInvalidOperationException()
        {
            var invalidData = "Invalid Data";

            Assert.Throws<InvalidOperationException>(() =>
            {
                SerializerFactory.CompareSerializedData<VersionObject>(invalidData, "xml", invalidData, "xml");
            });
        }
    }
};

