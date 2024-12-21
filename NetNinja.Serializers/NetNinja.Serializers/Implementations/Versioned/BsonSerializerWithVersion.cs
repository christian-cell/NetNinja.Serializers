using System.IO.Compression;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NetNinja.Serializers.Abstractions;

namespace NetNinja.Serializers.Implementations.Versioned
{
    public class BsonSerializerWithVersion<T> : ISerializer<T> where T : IVersioned
    {
        public string Serialize(T obj)
        {
            var bsonDocument = obj.ToBsonDocument();
            return bsonDocument.ToJson();
        }

        public byte[] SerializeCompressed(T obj)
        {
            var bsonData = obj.ToBson();
            using var memoryStream = new MemoryStream();
            using (var gzip = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzip.Write(bsonData, 0, bsonData.Length);
            }
            return memoryStream.ToArray();
        }

        public void SerializeToStream(T obj, Stream stream)
        {
            using var bsonWriter = new BsonBinaryWriter(stream);
            BsonSerializer.Serialize(bsonWriter, obj);
        }

        public string SerializeMessages(List<T> messages)
        {
            var bsonArray = new BsonArray();
            foreach (var message in messages)
            {
                bsonArray.Add(message.ToBsonDocument());
            }
            return bsonArray.ToJson();
        }

        public T Deserialize(string serializedData)
        {
            var bsonDocument = BsonDocument.Parse(serializedData);
            return BsonSerializer.Deserialize<T>(bsonDocument);
        }

        public T DeserializeFromStream(Stream stream)
        {
            using var bsonReader = new BsonBinaryReader(stream);
            return BsonSerializer.Deserialize<T>(bsonReader);
        }
        
        public T DeserializeCompressed(byte[] compressedData)
        {
            using var compressedStream = new MemoryStream(compressedData);
            using var gzip = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var memoryStream = new MemoryStream(); 

            gzip.CopyTo(memoryStream);
            memoryStream.Position = 0; 

            using var bsonReader = new BsonBinaryReader(memoryStream);
            return BsonSerializer.Deserialize<T>(bsonReader);
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

        public BsonDocument SerializeWithVersion(T obj, string version = null)
        {
            if (version != null)
            {
                obj.Version = version;
            }
            return obj.ToBsonDocument();
        }

        public (T deserializedObject, string version) DeserializeWithVersion(BsonDocument bsonData)
        {
            var deserializedObj = BsonSerializer.Deserialize<T>(bsonData);

            var versionExtracted = bsonData["Version"].AsString;

            return (deserializedObject: deserializedObj, version: versionExtracted);
        }
    }
};

