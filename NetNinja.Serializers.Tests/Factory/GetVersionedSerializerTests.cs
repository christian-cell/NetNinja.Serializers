using System;
using System.Collections.Generic;
using Xunit;

namespace NetNinja.Serializers.Tests.Factory
{
    public class GetVersionedSerializerTests
    {
        public interface IVersioned {}
        public interface ISerializer<T> {}

        public class MockVersionedObject : IVersioned {}
        public class MockSerializer<T> : ISerializer<T>
        {
            public T Data { get; set; }
        }

        private static readonly Dictionary<string, Func<Type, object>> VersionedSerializers = new();

        public static ISerializer<T> GetVersionedSerializer<T>(string format) where T : IVersioned
        {
            if (!VersionedSerializers.TryGetValue(format.ToUpper(), out var serializerFactory))
                throw new NotSupportedException($"Versioned serializer for '{format}' is not supported.");

            return (ISerializer<T>)serializerFactory(typeof(T));
        }

        [Fact]
        public void GetVersionedSerializer_ReturnsCorrectSerializer()
        {
            var format = "JSON";
            VersionedSerializers[format.ToUpper()] = type => new MockSerializer<MockVersionedObject>();

            var serializer = GetVersionedSerializer<MockVersionedObject>(format);

            Assert.NotNull(serializer);
            Assert.IsType<MockSerializer<MockVersionedObject>>(serializer);
        }

        [Fact]
        public void GetVersionedSerializer_FormatNotRegistered_ThrowsNotSupportedException()
        {
            var format = "YAML";

            var exception = Assert.Throws<NotSupportedException>(() => GetVersionedSerializer<MockVersionedObject>(format));
            Assert.Equal($"Versioned serializer for '{format}' is not supported.", exception.Message);
        }

        [Fact]
        public void GetVersionedSerializer_CaseInsensitiveFormat_ReturnsCorrectSerializer()
        {
            var format = "XML";
            VersionedSerializers[format.ToUpper()] = type => new MockSerializer<MockVersionedObject>();

            var serializer = GetVersionedSerializer<MockVersionedObject>("xml"); 

            Assert.NotNull(serializer);
            Assert.IsType<MockSerializer<MockVersionedObject>>(serializer);
        }

        [Fact]
        public void GetVersionedSerializer_InvalidCast_ThrowsInvalidCastException()
        {
            var format = "TEXT";
            VersionedSerializers[format.ToUpper()] = type => new object(); 

            Assert.Throws<InvalidCastException>(() => GetVersionedSerializer<MockVersionedObject>(format));
        }

        [Fact]
        public void RegisterAndRetrieveSerializer_Success()
        {
            var format = "JSON";
            Func<Type, object> serializerFactory = type => new MockSerializer<MockVersionedObject>();
            VersionedSerializers[format.ToUpper()] = serializerFactory;

            var serializer = GetVersionedSerializer<MockVersionedObject>(format);

            Assert.NotNull(serializer);
            Assert.IsType<MockSerializer<MockVersionedObject>>(serializer);
        }
    }
}