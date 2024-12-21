using System.IO.Compression;
using NetNinja.Serializers.Abstractions;
using Newtonsoft.Json;

namespace NetNinja.Serializers.Implementations.Versioned
{
    public class NewtonsoftSerializerWithVersion<T> : ISerializer<T> where T : IVersioned
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftSerializerWithVersion()
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, 
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };
        }

        public string Serialize(T obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
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
            
            var serialized = Serialize(obj);
            
            writer.Write(serialized);
            
            writer.Flush();
        }

        public string SerializeMessages(List<T> messages)
        {
            return JsonConvert.SerializeObject(messages, _settings);
        }

        public T Deserialize(string serializedData)
        {
            return JsonConvert.DeserializeObject<T>(serializedData, _settings);
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
            var deserializedObj = JsonConvert.DeserializeObject<T>(serializedData);

            string versionExtracted = deserializedObj.Version;

            return (deserializedObject: deserializedObj, version: versionExtracted);
        }
    }
};

