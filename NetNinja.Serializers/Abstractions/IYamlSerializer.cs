namespace NetNinja.Serializers.Abstractions
{
    public interface IYamlSerializer<T>
    {
          string SerializeToYaml(T obj);
          T DeserializeFromYaml(string yaml);
    }
};
