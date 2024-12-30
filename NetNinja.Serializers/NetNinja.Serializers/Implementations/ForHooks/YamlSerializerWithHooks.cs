using YamlDotNet.Serialization;

namespace NetNinja.Serializers.Implementations.ForHooks
{
    public class YamlSerializerWithHooks<T> : SerializerWithHooks<T>
    {
        public override string Serialize(T obj, bool? encrypt = false, string format = "")
        {
            if (BeforeSerialize != null)
            {
                var transformedObj = BeforeSerialize.Invoke(obj);
                
                if (transformedObj is not null)
                {
                    obj = transformedObj;
                }
            }

            var serializerBuilder = new SerializerBuilder();
            
            if (format.Equals("Compact", StringComparison.OrdinalIgnoreCase))
            {
                serializerBuilder.DisableAliases(); 
            }

            var serializer = serializerBuilder.Build();
            
            return serializer.Serialize(obj);
        }

        public override T Deserialize(string data, bool encrypt = false)
        {
            var deserializer = new DeserializerBuilder().Build();
            
            var obj = deserializer.Deserialize<T>(data);

            if (obj == null)
                throw new InvalidOperationException("The deserialized object is null.");

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
    }
};

