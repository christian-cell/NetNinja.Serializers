using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace NetNinja.Serializers.Implementations.ForDynamics
{
    public class JsonSerializerForDynamic
    {
        public string Serialize(object obj, bool indented = false)
        {
            var formatting = indented ? Formatting.Indented : Formatting.None;

            string serializedData = JsonConvert.SerializeObject(obj, (Newtonsoft.Json.Formatting)formatting);
            
            Console.WriteLine($"[Serialization] Object serialized of type: {obj.GetType().Name}, Data size: {serializedData.Length} bytes");
            
            return serializedData;
        }

        public object Deserialize(string data)
        {
            var deserializedObject = JsonConvert.DeserializeObject(data);
           
            Console.WriteLine($"[Deserialization] Object deserialized of dynamic type.");
            
            return deserializedObject!;
        }
    }
};

