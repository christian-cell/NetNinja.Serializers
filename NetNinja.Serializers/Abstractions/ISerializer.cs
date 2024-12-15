namespace NetNinja.Serializers.Abstractions
{
    public interface ISerializer<T>
    { 
        string Serialize(T obj);
        
        T Deserialize(string json);
        
        string SerializeMessages(List<T> messages);
        
        void SerializeToStream(T obj, Stream stream);
        
        T DeserializeFromStream(Stream stream);
        
        byte[] SerializeCompressed(T obj);
        
        T DeserializeCompressed(byte[] compressedData);
        
        bool ValidateSerializedData(string serializedData, string schema);
    }
};

