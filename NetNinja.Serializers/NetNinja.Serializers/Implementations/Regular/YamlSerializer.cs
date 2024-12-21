using System.Text;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NetNinja.Serializers.Implementations.Regular
{
    public class YamlSerializer<T> : ISerializer<T>
    {
        private readonly ISerializer _serializer; 
        private readonly IDeserializer _deserializer; 

        public YamlSerializer()
        {
            _serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) 
                .Build();

            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) 
                .Build();
        }

        #region Serialization Methods

        public string Serialize(T obj)
        {
            return _serializer.Serialize(obj);
        }

        public string SerializeMessages(List<T> messages)
        {
            return _serializer.Serialize(messages);
        }

        public void SerializeToStream(T obj, Stream stream)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                var yaml = Serialize(obj);
                streamWriter.Write(yaml);
            }
        }

        #endregion
        
        #region Deserialization Methods
        
        public T DeserializeFromStream(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var yaml = streamReader.ReadToEnd();
                return Deserialize(yaml);
            }
        }
        
        public T Deserialize(string yaml)
        {
            return _deserializer.Deserialize<T>(yaml);
        }
        
        #endregion

        #region Compressed Methods actions

        public byte[] SerializeCompressed(T obj)
        {
            var serializedData = Encoding.UTF8.GetBytes(Serialize(obj));
            return CompressionHelper.Compress(serializedData);
        }

        public T DeserializeCompressed(byte[] compressedData)
        {
            var decompressedData = CompressionHelper.Decompress(compressedData);
            var yaml = Encoding.UTF8.GetString(decompressedData);
            return Deserialize(yaml);
        }

        #endregion

        #region Validation Methods

        public bool ValidateSerializedData(string serializedData, string schema)
        {
            try
            {
                var yamlObject = _deserializer.Deserialize<object>(serializedData);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(yamlObject);

                var jsonSchema = JSchema.Parse(schema);

                var jsonData = JObject.Parse(json);
                return jsonData.IsValid(jsonSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"YAML Validation Error: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
};

