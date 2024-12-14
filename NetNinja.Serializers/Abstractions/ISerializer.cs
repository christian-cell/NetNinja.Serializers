namespace NetNinja.Serializers.Abstractions
{
    public interface ISerializer<T>
    {
        string SerializeMessages(List<T> messages);
    }
};

