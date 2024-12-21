using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Implementations.Versioned;

namespace NetNinja.Serializers.Factories
{
    public static class SerializerFactory
    {
        private static readonly ConcurrentDictionary<string, Func<Type, object>> VersionedSerializers = new();

        static SerializerFactory()
        {
            RegisterVersioned("JSON", type => Activator.CreateInstance(typeof(SystemTextSerializerWithVersion<>).MakeGenericType(type))!);
            RegisterVersioned("NEWTONSOFT_JSON", type => Activator.CreateInstance(typeof(NewtonsoftSerializerWithVersion<>).MakeGenericType(type))!);
            RegisterVersioned("XML", type => Activator.CreateInstance(typeof(XmlSerializerWithVersion<>).MakeGenericType(type))!);
            RegisterVersioned("YAML", type => Activator.CreateInstance(typeof(YamlSerializerWithVersion<>).MakeGenericType(type))!);
            RegisterVersioned("BSON", type => Activator.CreateInstance(typeof(BsonSerializerWithVersion<>).MakeGenericType(type))!);
        }

        public static void RegisterVersioned(string format, Func<Type, object> serializerFactory)
        {
            if (VersionedSerializers.ContainsKey(format.ToUpper()))
                throw new InvalidOperationException($"Versioned serializer for '{format}' is already registered.");

            VersionedSerializers[format.ToUpper()] = serializerFactory;
        }

        public static ISerializer<T> GetVersionedSerializer<T>(string format) where T : IVersioned
        {
            if (!VersionedSerializers.TryGetValue(format.ToUpper(), out var serializerFactory))
                throw new NotSupportedException($"Versioned serializer for '{format}' is not supported.");

            return (ISerializer<T>)serializerFactory(typeof(T));
        }

        public static IEnumerable<string> GetAvailableVersionedFormats()
        {
            return VersionedSerializers.Keys;
        }
        
        public static string ConvertBetweenFormats<T>(string sourceData, string sourceFormat, string targetFormat) where T : IVersioned
        {
            var sourceSerializer = GetVersionedSerializer<T>(sourceFormat);

            var targetSerializer = GetVersionedSerializer<T>(targetFormat);

            var deserializedObject = sourceSerializer.Deserialize(sourceData);

            var convertedData = targetSerializer.Serialize(deserializedObject);

            return convertedData;
        }
        
        public static bool CompareSerializedData<T>(string data1, string format1, string data2, string format2) where T : IVersioned
        {
            var serializer1 = GetVersionedSerializer<T>(format1);
            
            var object1 = serializer1.Deserialize(data1);

            var serializer2 = GetVersionedSerializer<T>(format2);
            
            var object2 = serializer2.Deserialize(data2);

            return AreObjectsEqual(object1, object2);
        }

        private static bool AreObjectsEqual<T>(T obj1, T obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                return obj1 == null && obj2 == null;
            }

            var json1 = JsonSerializer.Serialize(obj1);
            
            var json2 = JsonSerializer.Serialize(obj2);

            return string.Equals(json1, json2, StringComparison.Ordinal);
        }
        
        public static List<string> GetDifferences<T>(string data1, string format1, string data2, string format2) where T : IVersioned
        {
            var serializer1 = GetVersionedSerializer<T>(format1);
            
            var object1 = serializer1.Deserialize(data1);

            var serializer2 = GetVersionedSerializer<T>(format2);
           
            var object2 = serializer2.Deserialize(data2);

            return GetObjectDifferences(object1, object2);
        }

        private static List<string> GetObjectDifferences<T>(T obj1, T obj2)
        {
            var differences = new List<string>();

            if (obj1 == null && obj2 == null)
            {
                return differences; 
            }

            if (obj1 == null)
            {
                differences.Add("Object 1 is null while Object 2 has a value.");
               
                return differences;
            }

            if (obj2 == null)
            {
                differences.Add("Object 2 is null while Object 1 has a value.");
               
                return differences;
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value1 = property.GetValue(obj1);
                
                var value2 = property.GetValue(obj2);

                if (!Equals(value1, value2))
                {
                    differences.Add($"Property '{property.Name}' differs: '{value1}' vs. '{value2}'");
                }
            }

            return differences;
        }
    }
}