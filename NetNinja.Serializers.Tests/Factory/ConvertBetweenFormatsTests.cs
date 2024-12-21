using System;
using System.Collections.Generic;
using Xunit;

namespace NetNinja.Serializers.Tests.Factory
{
    public class ConvertBetweenFormatsTests
    {
        public interface IVersioned {}
        public interface ISerializer<T>
        {
            T Deserialize(string data);
            string Serialize(T obj);
        }

        public class MockVersionedObject : IVersioned
        {
            public string Value { get; set; }
        }

        public class MockSerializer<T> : ISerializer<T>
        {
            private readonly Func<string, T> _deserializeFunc;
            private readonly Func<T, string> _serializeFunc;

            public MockSerializer(Func<string, T> deserializeFunc, Func<T, string> serializeFunc)
            {
                _deserializeFunc = deserializeFunc;
                _serializeFunc = serializeFunc;
            }

            public T Deserialize(string data) => _deserializeFunc(data);
            public string Serialize(T obj) => _serializeFunc(obj);
        }

        private static readonly Dictionary<string, Func<Type, object>> VersionedSerializers = new();

        public static string ConvertBetweenFormats<T>(string sourceData, string sourceFormat, string targetFormat) where T : IVersioned
        {
            var sourceSerializer = GetVersionedSerializer<T>(sourceFormat);
            var targetSerializer = GetVersionedSerializer<T>(targetFormat);

            var deserializedObject = sourceSerializer.Deserialize(sourceData);
            var convertedData = targetSerializer.Serialize(deserializedObject);

            return convertedData;
        }

        public static ISerializer<T> GetVersionedSerializer<T>(string format) where T : IVersioned
        {
            if (!VersionedSerializers.TryGetValue(format.ToUpper(), out var serializerFactory))
                throw new NotSupportedException($"Versioned serializer for '{format}' is not supported.");

            return (ISerializer<T>)serializerFactory(typeof(T));
        }

        [Fact]
        public void ConvertBetweenFormats_SuccessfulConversion()
        {
            var sourceFormat = "JSON";
            var targetFormat = "XML";
            var sourceData = "{\"Value\":\"Test\"}";
            var expectedData = "<Value>Test</Value>";

            VersionedSerializers[sourceFormat.ToUpper()] = type => new MockSerializer<MockVersionedObject>(
                data => new MockVersionedObject { Value = "Test" },
                obj => "{\"Value\":\"" + (obj as MockVersionedObject).Value + "\"}"
            );

            VersionedSerializers[targetFormat.ToUpper()] = type => new MockSerializer<MockVersionedObject>(
                data => new MockVersionedObject { Value = "Test" },
                obj => "<Value>" + (obj as MockVersionedObject).Value + "</Value>"
            );

            var result = ConvertBetweenFormats<MockVersionedObject>(sourceData, sourceFormat, targetFormat);

            Assert.Equal(expectedData, result);
        }

        [Fact]
        public void ConvertBetweenFormats_SourceFormatNotRegistered_ThrowsNotSupportedException()
        {
            var sourceFormat = "YAML";
            var targetFormat = "XML";
            var sourceData = "Some data";

            var exception = Assert.Throws<NotSupportedException>(() => ConvertBetweenFormats<MockVersionedObject>(sourceData, sourceFormat, targetFormat));
            Assert.Equal($"Versioned serializer for '{sourceFormat}' is not supported.", exception.Message);
        }

        [Fact]
        public void ConvertBetweenFormats_TargetFormatNotRegistered_ThrowsNotSupportedException()
        {
            var sourceFormat = "JSON";
            var targetFormat = "BSON";
            var sourceData = "{\"Value\":\"Test\"}";

            VersionedSerializers[sourceFormat.ToUpper()] = type => new MockSerializer<MockVersionedObject>(
                data => new MockVersionedObject { Value = "Test" },
                obj => "{\"Value\":\"" + (obj as MockVersionedObject).Value + "\"}"
            );

            var exception = Assert.Throws<NotSupportedException>(() => ConvertBetweenFormats<MockVersionedObject>(sourceData, sourceFormat, targetFormat));
            Assert.Equal($"Versioned serializer for '{targetFormat}' is not supported.", exception.Message);
        }

        [Fact]
        public void ConvertBetweenFormats_DeserializationFails_ThrowsException()
        {
            var sourceFormat = "JSON";
            var targetFormat = "XML";
            var sourceData = "{\"Value\":\"Test\"}";

            VersionedSerializers[sourceFormat.ToUpper()] = type => new MockSerializer<MockVersionedObject>(
                data => throw new InvalidOperationException("Deserialization failed."),
                obj => "{\"Value\":\"" + (obj as MockVersionedObject).Value + "\"}"
            );

            VersionedSerializers[targetFormat.ToUpper()] = type => new MockSerializer<MockVersionedObject>(
                data => new MockVersionedObject { Value = "Test" },
                obj => "<Value>" + (obj as MockVersionedObject).Value + "</Value>"
            );

            var exception = Assert.Throws<InvalidOperationException>(() => ConvertBetweenFormats<MockVersionedObject>(sourceData, sourceFormat, targetFormat));
            Assert.Equal("Deserialization failed.", exception.Message);
        }

        [Fact]
        public void ConvertBetweenFormats_SerializationFails_ThrowsException()
        {
            var sourceFormat = "JSON";
            var targetFormat = "XML";
            var sourceData = "{\"Value\":\"Test\"}";

            VersionedSerializers[sourceFormat.ToUpper()] = type => new MockSerializer<MockVersionedObject>(
                data => new MockVersionedObject { Value = "Test" },
                obj => throw new InvalidOperationException("Serialization failed.")
            );

            VersionedSerializers[targetFormat.ToUpper()] = type => new MockSerializer<MockVersionedObject>(
                data => new MockVersionedObject { Value = "Test" },
                obj => throw new InvalidOperationException("Serialization failed.")
            );

            var exception = Assert.Throws<InvalidOperationException>(() => ConvertBetweenFormats<MockVersionedObject>(sourceData, sourceFormat, targetFormat));
            Assert.Equal("Serialization failed.", exception.Message);
        }
    }
}