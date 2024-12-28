using System.Text;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace NetNinja.Serializers.Implementations.Regular
{
    public class NewtonsoftSerializer<T>: ISerializer<T>
    {
        private readonly Newtonsoft.Json.JsonSerializerSettings _newtonsoftSettings;
        
        private readonly SerializerOptions _serializerOptions;

        public NewtonsoftSerializer(SerializerOptions serializerOptions, JsonSerializerSettings? newtonsoftSettings = null)
        {
            _serializerOptions = serializerOptions;
            _newtonsoftSettings = newtonsoftSettings ?? new JsonSerializerSettings();
        }

        #region Serialization Methods

        public string Serialize(T obj)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            return JsonConvert.SerializeObject(obj, _serializerOptions.NewtonSoftJsonSettings);
        }

        public string SerializeMessages(List<T> messages)
        {
            return JsonConvert.SerializeObject(messages, _serializerOptions.NewtonSoftJsonSettings);
        }

        public void SerializeToStream(T obj, Stream stream)
        {
            using var streamWriter = new StreamWriter(stream);

            using var jsonWriter = new JsonTextWriter(streamWriter);
            
            var serializer = JsonSerializer.Create(_serializerOptions.NewtonSoftJsonSettings);
            
            serializer.Serialize(jsonWriter, obj);
        }

        #endregion

        #region Deserialization Methods

        public T DeserializeFromStream(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            
            using var jsonReader = new JsonTextReader(streamReader);
            
            var serializer = JsonSerializer.Create(_serializerOptions.NewtonSoftJsonSettings);
            
            return serializer.Deserialize<T>(jsonReader) ?? throw new InvalidOperationException();
        }
        
        public T Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _serializerOptions.NewtonSoftJsonSettings) ?? throw new InvalidOperationException();
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
                JSchema jsonSchema = JSchema.Parse(schema);

                JObject jsonData = JObject.Parse(serializedData);

                return jsonData.IsValid(jsonSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in validation: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
};

