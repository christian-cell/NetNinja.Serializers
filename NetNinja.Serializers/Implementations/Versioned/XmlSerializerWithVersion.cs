using System.IO.Compression;
using System.Xml.Serialization;
using NetNinja.Serializers.Abstractions;

namespace NetNinja.Serializers.Implementations.Versioned
{
    public class XmlSerializerWithVersion<T> : ISerializer<T> where T : IVersioned
    {
        public string Serialize(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringWriter textWriter = new StringWriter();
            serializer.Serialize(textWriter, obj);
            return textWriter.ToString();
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
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, obj);
        }

        public string SerializeMessages(List<T> messages)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using StringWriter textWriter = new StringWriter();
            serializer.Serialize(textWriter, messages);
            return textWriter.ToString();
        }

        public T Deserialize(string serializedData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringReader textReader = new StringReader(serializedData);
            return (T)serializer.Deserialize(textReader);
        }

        public T DeserializeFromStream(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
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

