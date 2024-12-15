using System.IO.Compression;
using System.Text;
using System.Text.Json;
using NetNinja.Serializers.Abstractions;

namespace NetNinja.Serializers.Implementations.Versioned
{
    public class SystemTextSerializerWithVersion<T> : ISerializer<T> where T : IVersioned
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextSerializerWithVersion()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public string Serialize(T obj)
        {
            return JsonSerializer.Serialize(obj, _options);
        }

        public byte[] SerializeCompressed(T obj)
        {
            var serialized = Serialize(obj);
            var bytes = Encoding.UTF8.GetBytes(serialized);
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
            var serialized = Serialize(obj);
            writer.Write(serialized);
            writer.Flush();
        }

        public string SerializeMessages(List<T> messages)
        {
            return JsonSerializer.Serialize(messages, _options);
        }

        public T Deserialize(string serializedData)
        {
            return JsonSerializer.Deserialize<T>(serializedData, _options);
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
            return (deserializedObj, deserializedObj.Version);
        }
    }
};

