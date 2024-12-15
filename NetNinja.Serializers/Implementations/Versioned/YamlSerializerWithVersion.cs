using System.IO.Compression;
using NetNinja.Serializers.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NetNinja.Serializers.Implementations.Versioned
{
    public class YamlSerializerWithVersion<T> : ISerializer<T> where T : IVersioned
    {
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;

        public YamlSerializerWithVersion()
        {
            _serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }

        public string Serialize(T obj)
        {
            return _serializer.Serialize(obj);
        }

        public byte[] SerializeCompressed(T obj)
        {
            var serialized = Serialize(obj);
            var bytes = System.Text.Encoding.UTF8.GetBytes(serialized);
            using var memoryStream = new MemoryStream();
            using (var gzip = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }
            return memoryStream.ToArray();
        }

        public void SerializeToStream(T obj, Stream stream)
        {
            var writer = new StreamWriter(stream);
            writer.Write(Serialize(obj));
            writer.Flush();
        }

        public string SerializeMessages(List<T> messages)
        {
            return _serializer.Serialize(messages);
        }

        public T Deserialize(string serializedData)
        {
            return _deserializer.Deserialize<T>(serializedData);
        }

        public T DeserializeFromStream(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var serializedData = reader.ReadToEnd();
            return Deserialize(serializedData);
        }

        public T DeserializeCompressed(byte[] compressedData)
        {
            using var memoryStream = new MemoryStream(compressedData);
            using var gzip = new GZipStream(memoryStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip);
            var serializedData = reader.ReadToEnd();
            return Deserialize(serializedData);
        }

        public bool ValidateSerializedData(string serializedData, string schema)
        {
            try
            {
                var deserialized = Deserialize(serializedData);
                return deserialized != null;
            }
            catch
            {
                return false;
            }
        }

        public string SerializeWithVersion(T obj, string version = null)
        {
            if (version != null)
            {
                obj.Version = version;
            }
            return Serialize(obj);
        }

        public (T deserializedObject, string version) DeserializeWithVersion(string serializedData)
        {
            var deserializedObj = Deserialize(serializedData);
            return (deserializedObject: deserializedObj, version: deserializedObj.Version);
        }
    }
};

