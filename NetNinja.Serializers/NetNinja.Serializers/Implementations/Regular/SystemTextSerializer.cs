using System.Text;
using System.Text.Json;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace NetNinja.Serializers.Implementations.Regular
{
    public class SystemTextSerializer<T>: ISerializer<T>
    {
        
        private readonly JsonSerializerOptions _options;

        public SystemTextSerializer(JsonSerializerOptions? options = null)
        {
            _options = options ?? new JsonSerializerOptions();
        }

        #region Serialization Methods

        public string SerializeMessages(List<T> messages)
        {
            return JsonSerializer.Serialize(messages);
        }

        public string Serialize(T obj)
        {
            return JsonSerializer.Serialize(obj, _options);
        }
        
        public void SerializeToStream(T obj, Stream stream)
        {
            JsonSerializer.Serialize(stream, obj, _options);
        }

        #endregion

        #region Deserialization Methods

        public T DeserializeFromStream(Stream stream)
        {
            return JsonSerializer.Deserialize<T>(stream, _options) ?? throw new InvalidOperationException();
        }
        
        public T Deserialize(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options) ?? throw new InvalidOperationException();
        }

        #endregion
        
        #region Compressed Methods
        
        public byte[] SerializeCompressed(T obj)
        {
            var serializedData = Encoding.UTF8.GetBytes(Serialize(obj));
            return CompressionHelper.Compress(serializedData);
        }

        public T DeserializeCompressed(byte[] compressedData)
        {
            var decompressedData = CompressionHelper.Decompress(compressedData);
            var json = Encoding.UTF8.GetString(decompressedData);
            return Deserialize(json);
        }
        
        #endregion

        #region Validation Methods

        public bool ValidateSerializedData(string serializedData, string schema)
        {
            try
            {
                // Validando nuevamente con JSON Schema
                var jsonSchema = JSchema.Parse(schema);
                var jsonData = JObject.Parse(serializedData);
                return jsonData.IsValid(jsonSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System.Text.Json Validation Error: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
};
