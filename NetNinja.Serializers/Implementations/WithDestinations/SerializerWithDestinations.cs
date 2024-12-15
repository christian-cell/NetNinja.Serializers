using NetNinja.Serializers.Implementations.ForHooks;

namespace NetNinja.Serializers.Implementations.WithDestinations
{
    public class SerializerWithDestinations<T>
    {
        private readonly JsonSerializerWithHooks<T> _jsonSerializer;

        public SerializerWithDestinations(JsonSerializerWithHooks<T> jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public void SerializeToFile(T obj, string filePath, string format = "Compact")
        {
            var serializedData = _jsonSerializer.Serialize(obj, format);
            
            File.WriteAllText(filePath, serializedData); 
            
            Console.WriteLine($"Data serialized and saved to file: {filePath}");
        }

        public T DeserializeFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var serializedData = File.ReadAllText(filePath); 
            
            var deserializedObject = _jsonSerializer.Deserialize(serializedData);
            
            Console.WriteLine($"Data deserialized from file: {filePath}");
            
            return deserializedObject;
        }
    }
};