using NetNinja.Serializers.Abstractions;

namespace NetNinja.Serializers.Registries
{
    public static class SerializerRegistry
    {
        private static readonly Dictionary<string, Func<object>> Serializers = new();

        public static void Register<T>(string format, ISerializer<T> serializer)
        {
            if (Serializers.ContainsKey(format.ToUpper()))
            {
                throw new InvalidOperationException($"A serializer for format '{format}' is already registered.");
            }

            Serializers[format.ToUpper()] = () => serializer;
        }

        public static ISerializer<T> GetSerializer<T>(string format)
        {
            if (!Serializers.TryGetValue(format.ToUpper(), out var serializerFactory))
            {
                throw new NotSupportedException($"The serialization format '{format}' is not supported.");
            }

            return (ISerializer<T>)serializerFactory();
        }

        public static IEnumerable<string> GetRegisteredFormats()
        {
            return Serializers.Keys;
        }
    }
};

