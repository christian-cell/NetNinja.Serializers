using System.Text;
using System.Text.Json;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace NetNinja.Serializers.Implementations.Regular
{
    public class SystemTextSerializer<T>: ISerializer<T>
    {
        
        private readonly JsonSerializerOptions _options;
        private readonly SerializerOptions _serializerOptions;
        public SerializerOptions SerializerOptions;

        public SystemTextSerializer(JsonSerializerOptions? options = null, SerializerOptions? serializerOptions = null)
        {
            _options = options ?? new JsonSerializerOptions();
            _serializerOptions = serializerOptions ?? new SerializerOptions();
        }

        #region Serialization Methods

        public string SerializeMessages(List<T> messages)
        {
            if(messages == null) throw new ArgumentNullException(nameof(messages));
            
            return JsonSerializer.Serialize(messages);
        }

        public string Serialize(T obj)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            
            return JsonSerializer.Serialize(obj, _options);
        }
        
        public void SerializeToStream(T obj, Stream stream)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if(stream == null) throw new ArgumentNullException(nameof(stream));

            JsonSerializer.Serialize(stream, obj, _options);
        }

        #endregion

        #region Deserialization Methods

        public T DeserializeFromStream(Stream stream)
        {
            if(stream == null) throw new ArgumentNullException(nameof(stream));
            
            return JsonSerializer.Deserialize<T>(stream, _options) ?? throw new InvalidOperationException();
        }
        
        public T Deserialize(string jsonStr)
        {
            if( string.IsNullOrEmpty(jsonStr) )throw new ArgumentNullException(nameof(jsonStr));
            
            return JsonSerializer.Deserialize<T>(jsonStr, _options) ?? throw new InvalidOperationException();
        }

        #endregion
        
        #region Compressed Methods
        
        public byte[] SerializeCompressed(T obj)
        {

            if (obj == null) throw new ArgumentNullException(nameof(obj));
            
            var serializedData = Encoding.UTF8.GetBytes(Serialize(obj));
            return CompressionHelper.Compress(serializedData);
        }

        public T DeserializeCompressed(byte[] compressedData)
        {
            if (compressedData == null) throw new ArgumentNullException(nameof(compressedData));
            
            var decompressedData = CompressionHelper.Decompress(compressedData);
            var json = Encoding.UTF8.GetString(decompressedData);
            return Deserialize(json);
        }
        
        #endregion

        #region Validation Methods

        public bool ValidateSerializedData(string serializedData, string schema)
        {
            if (string.IsNullOrEmpty(serializedData)) throw new ArgumentNullException(nameof(serializedData));
            if (string.IsNullOrEmpty(schema)) throw new ArgumentNullException(nameof(schema));
            
            try
            {
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

