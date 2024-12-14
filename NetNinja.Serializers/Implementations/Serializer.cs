using System.Text.Json;
using NetNinja.Serializers.Abstractions;

namespace NetNinja.Serializers.Implementations
{
    public class Serializer<T>: ISerializer<T>
    {

        public string SerializeMessages(List<T> messages)
        {
            return JsonSerializer.Serialize(messages);
        }
    }
};

