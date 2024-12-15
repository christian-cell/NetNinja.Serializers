using NetNinja.Serializers.Abstractions;

namespace NetNinja.Serializers.Extensions
{
    public static class SerializerExtensions
    {
        public static string Serialize<T>(this T obj, ISerializer<T> serializer)
        {
            return serializer.Serialize(obj);
        }

        public static T Deserialize<T>(this string serializedData, ISerializer<T> serializer)
        {
            return serializer.Deserialize(serializedData);
        }

        public static string SerializeMessages<T>(this List<T> messages, ISerializer<T> serializer)
        {
            return serializer.SerializeMessages(messages);
        }
    }

    public static class StreamSerializerExtensions
    {
        public static void SerializeToStream<T>(this T obj, Stream stream, ISerializer<T> serializer)
        {
            serializer.SerializeToStream(obj, stream);
        }

        public static T DeserializeFromStream<T>(this Stream stream, ISerializer<T> serializer)
        {
            return serializer.DeserializeFromStream(stream);
        }
    }
};

