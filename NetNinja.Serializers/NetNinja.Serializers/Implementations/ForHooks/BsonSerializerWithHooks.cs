using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace NetNinja.Serializers.Implementations.ForHooks
{
    public class BsonSerializerWithHooks<T> : SerializerWithHooks<T>
    {
        #region Sync Methods
        public override string Serialize(T obj, string format = "Compact")
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

            var jsonWriterSettings = format.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                ? new JsonWriterSettings { Indent = true }
                : new JsonWriterSettings { Indent = false };

            return bsonDocument.ToJson(jsonWriterSettings);
        }

        public override T Deserialize(string data)
        {
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
        
        public override string CombineSerialized(IEnumerable<T> objects)
        {
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

            return bsonArray.ToJson(); 
        }

        public override IEnumerable<T> SplitSerialized(string combinedSerialized)
        {
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

        public override async Task<string> SerializeAsync(T obj, string format = "Compact")
        {
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

                var jsonWriterSettings = format.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                    ? new JsonWriterSettings { Indent = true }
                    : new JsonWriterSettings { Indent = false };

                return bsonDocument.ToJson(jsonWriterSettings);
            });
        }

        #endregion
    }
};