using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace NetNinja.Serializers.Implementations.Regular
{
    public class BsonSerializer<T> : NetNinja.Serializers.Abstractions.IBsonSerializer<T>, ISerializer<T>
    {
        #region Serialization Methods

        public byte[] SerializeToBson(T obj)
        {
            return obj.ToBson();
        }
        
        public string Serialize(T obj)
        {
            var bsonBytes = SerializeToBson(obj);
            return bsonBytes.ToHexString(); 
        }
        
        public void SerializeToStream(T obj, Stream stream)
        {
            var writerSettings = new BsonBinaryWriterSettings();
            using var bsonWriter = new BsonBinaryWriter(stream, writerSettings);
            BsonSerializer.Serialize(bsonWriter, obj);
        }
        
        public string SerializeMessages(List<T> messages)
        {
            var bsonDocument = new BsonDocument
            {
                { "Messages", new BsonArray(messages.Select(obj => obj.ToBsonDocument())) }
            };

            return bsonDocument.ToBson().ToHexString();
        }

        #endregion

        #region Deserialization Methods
        
        public T DeserializeFromBson(byte[] bson)
        {
            return BsonSerializer.Deserialize<T>(bson);
        }

        

        public T Deserialize(string serializedData)
        {
            var bsonBytes = serializedData.HexToBytes(); 
            return DeserializeFromBson(bsonBytes);
        }

        

        public T DeserializeFromStream(Stream stream)
        {
            using var bsonReader = new BsonBinaryReader(stream);
            return BsonSerializer.Deserialize<T>(bsonReader);
        }
        
        #endregion

        #region Compressed Methods

        public byte[] SerializeCompressed(T obj)
        {
            var bsonData = SerializeToBson(obj);
            return CompressionHelper.Compress(bsonData);
        }

        public T DeserializeCompressed(byte[] compressedData)
        {
            var decompressedBson = CompressionHelper.Decompress(compressedData);
            return DeserializeFromBson(decompressedBson);
        }

        #endregion

        #region Validation Methods

        public bool ValidateSerializedData(string serializedData, string schema)
        {
            try
            {
                var jsonSchema = JSchema.Parse(schema);

                // Convertir BSON string a JSON para validación
                var dataAsJson = BsonDocument.Parse(serializedData).ToJson();

                var jsonData = JObject.Parse(dataAsJson);
                return jsonData.IsValid(jsonSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BSON Validation Error: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
};

