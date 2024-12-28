using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Helpers;

namespace NetNinja.Serializers.Implementations.ForHooks
{
    public class BsonSerializerWithHooks<T> : SerializerWithHooks<T>
    {
        private SerializerOptions _serializerOptions;
        private readonly EncryptionHelper _encryptionHelper;
        
        public BsonSerializerWithHooks(SerializerOptions serializerOptions, EncryptionHelper encryptionHelper)
        {
            _serializerOptions = serializerOptions;
            _encryptionHelper = encryptionHelper;
        }
        
        #region Sync Methods
        public override string Serialize(T obj, bool? encrypt = null , string format = "")
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "The object to serialize cannot be null.");
    
            var myFormat = !string.IsNullOrEmpty(format) ? format : _serializerOptions.DefaultFormat ?? "Compact";
    
            var useEncryption = encrypt ?? _serializerOptions.EnableEncryption;
    
            if (BeforeSerialize != null)
            {
                var transformedObj = BeforeSerialize.Invoke(obj);
                if (transformedObj is not null)
                {
                    obj = transformedObj;
                }
            }

            var bsonDocument = obj.ToBsonDocument();

            var jsonWriterSettings = myFormat.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                ? new JsonWriterSettings { Indent = true }
                : new JsonWriterSettings { Indent = false };

            var serializedData = bsonDocument.ToJson(jsonWriterSettings);

            if (useEncryption) serializedData = _encryptionHelper.Encrypt(serializedData);
    
            return serializedData;
        }

        public override T Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data), "The data to deserialize cannot be null or empty.");
            
            var bsonDocument = BsonDocument.Parse(data);
            var obj = BsonSerializer.Deserialize<T>(bsonDocument);

            if (obj == null)
                throw new InvalidOperationException("the deserialized object is null.");

            if (AfterDeserialize != null)
            {
                var transformedObj = AfterDeserialize.Invoke(obj);
                if (transformedObj is not null)
                {
                    return transformedObj;
                }
            }
            
            return obj;
        }
        
        public override string CombineSerialized(IEnumerable<T> objects, bool? encrypt = null , string format = "")
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects), "The objects to serialize cannot be null.");

            var myFormat = !string.IsNullOrEmpty(format) ? format : _serializerOptions.DefaultFormat ?? "Compact";
    
            var useEncryption = encrypt ?? _serializerOptions.EnableEncryption;
            
            var bsonArray = new BsonArray(objects.Select(obj =>
            {
                if (BeforeSerialize != null)
                {
                    var transformedObj = BeforeSerialize.Invoke(obj);
                    if (transformedObj is not null)
                    {
                        obj = transformedObj;
                    }
                }
                return obj.ToBsonDocument(); 
            }));
            
            var jsonWriterSettings = myFormat.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                ? new JsonWriterSettings { Indent = true }
                : new JsonWriterSettings { Indent = false };

            var serializedData = bsonArray.ToJson(jsonWriterSettings);
            
            if (useEncryption) serializedData = _encryptionHelper.Encrypt(serializedData);
    
            return serializedData;
        }

        public override IEnumerable<T> SplitSerialized(string combinedSerialized)
        {
            if (string.IsNullOrEmpty(combinedSerialized)) throw new ArgumentNullException(nameof(combinedSerialized), "The combined serialized data cannot be null or empty.");
            
            var bsonArray = BsonSerializer.Deserialize<BsonArray>(combinedSerialized); 

            return bsonArray.Select(bsonValue =>
            {
                var obj = BsonSerializer.Deserialize<T>(bsonValue.AsBsonDocument);

                if (AfterDeserialize != null)
                {
                    var transformedObj = AfterDeserialize.Invoke(obj);
                    if (transformedObj is not null)
                    {
                        return transformedObj;
                    }
                }
            
                return obj;
            });
        }
        #endregion

        #region Async Methods

        public override async Task<string> SerializeAsync(T obj, bool? encrypt = null, string format = "")
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), "The object to serialize cannot be null.");
            
            var myFormat = !string.IsNullOrEmpty(format) ? format : _serializerOptions.DefaultFormat ?? "Compact";
            
            var useEncryption = encrypt ?? _serializerOptions.EnableEncryption;
            
            return await Task.Run(() =>
            {
                if (BeforeSerialize != null)
                {
                    var transformedObj = BeforeSerialize.Invoke(obj);
                    if (transformedObj is not null)
                    {
                        obj = transformedObj;
                    }
                }

                var bsonDocument = obj.ToBsonDocument();

                var jsonWriterSettings = myFormat.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                    ? new JsonWriterSettings { Indent = true }
                    : new JsonWriterSettings { Indent = false };

                var serializedData = bsonDocument.ToJson(jsonWriterSettings);
                
                if (useEncryption) serializedData = _encryptionHelper.Encrypt(serializedData);

                return serializedData;
            });
        }

        #endregion
    }
};