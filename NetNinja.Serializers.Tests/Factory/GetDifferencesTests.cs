using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Factories;
namespace NetNinja.Serializers.Tests.Factory
{
    public class GetDifferencesTests
    {
        [Theory]
        [InlineData("{\"Version\":\"1.0\",\"Name\":\"Test\"}", "json", "{\"Version\":\"1.0\",\"Name\":\"Test\"}", "json", true)]
        [InlineData("{\"Version\":\"1.0\",\"Name\":\"Test\"}", "json", "<VersionObject><Version>1.0</Version><Name>Test</Name></VersionObject>", "xml", true)]
        [InlineData("{\"Version\":\"1.0\",\"Name\":\"Test\"}", "json", "{\"Version\":\"2.0\",\"Name\":\"Test\"}", "json", false)]
        public void Test_GetDifferences_WithSerializedData(string data1, string format1, string data2, string format2, bool shouldMatch)
        {
            // Act
            var differences = SerializerFactory.GetDifferences<VersionObject>(data1, format1, data2, format2);

            // Assert
            Assert.Equal(shouldMatch, !differences.Any());
        }
    }
};

