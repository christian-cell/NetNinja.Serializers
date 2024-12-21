namespace NetNinja.Serializers.Abstractions
{
    public interface IBsonSerializer<T>
    {
        byte[] SerializeToBson(T obj);
        T DeserializeFromBson(byte[] bson);
    }
};