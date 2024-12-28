using Microsoft.Extensions.Logging;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Helpers;
using NetNinja.Serializers.Implementations.ForHooks;

namespace NetNinja.Serializers.Implementations.WithDestinations
{
    public class SerializerWithDestinations<T>
    {
        private readonly JsonSerializerWithHooks<T> _jsonSerializer;
        private readonly ILogger<SerializerWithDestinations<T>> _logger;

        public SerializerWithDestinations(
            ILogger<SerializerWithDestinations<T>> logger, 
            ILogger<JsonSerializerWithHooks<T>> jsonLogger,
            SerializerOptions options
            )
        {
            _logger = logger;

            _jsonSerializer = new JsonSerializerWithHooks<T>(
                new EncryptionHelper(options.EncryptionConfiguration),
                jsonLogger,
                options.NewtonSoftJsonSettings,
                options.EnableEncryption
            );
        }

        public void SerializeToFile(T obj, string filePath, string format = null)
        {
            try
            {
                var actualFormat = format ?? "Compact";
                var serializedData = _jsonSerializer.Serialize(obj,false , actualFormat);

                File.WriteAllText(filePath, serializedData);
                _logger.LogInformation($"Data serialized and saved to file: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during serialization: {ex.Message}");
                throw;
            }
        }

        public T DeserializeFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogInformation($"File not found: {filePath}");
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                _logger.LogInformation($"Reading data from file: {filePath}");
                var serializedData = File.ReadAllText(filePath);

                _logger.LogInformation($"Deserializing data from file: {filePath}");
                var deserializedObject = _jsonSerializer.Deserialize(serializedData);

                _logger.LogInformation($"Data deserialized successfully from file: {filePath}");
                return deserializedObject;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during deserialization from file: {filePath}. Exception: {ex.Message}");
                throw;
            }
        }
    }
};