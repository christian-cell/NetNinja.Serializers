namespace NetNinja.Serializers.Abstractions
{
    public interface IXmlSerializer<T>
    { 
        string SerializeToXml(T obj);
        T DeserializeFromXml(string xml);
    };
};

