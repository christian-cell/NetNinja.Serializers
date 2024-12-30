using Microsoft.Extensions.Logging;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Helpers;
using NetNinja.Serializers.Implementations.ForHooks;
using NetNinja.Serializers.Responses;

namespace NetNinja.Serializers.Implementations.WithDestinations
{
    public class SerializerWithDestinations<T>
    {
        private readonly JsonSerializerWithHooks<T> _jsonSerializerWithHooks;
        private readonly ILogger<SerializerWithDestinations<T>> _logger;
        private readonly SerializerOptions _serializerOptions;

        public SerializerWithDestinations(
            ILogger<SerializerWithDestinations<T>> logger, 
            ILogger<JsonSerializerWithHooks<T>> jsonLogger,
            SerializerOptions serializerOptions
            )
        {
            _logger = logger;
            _serializerOptions = serializerOptions;
            _jsonSerializerWithHooks = new JsonSerializerWithHooks<T>(
                new EncryptionHelper(serializerOptions.EncryptionConfiguration),
                jsonLogger,
                serializerOptions,
                serializerOptions.NewtonSoftJsonSettings
            );
        }

        public BaseResponse SerializeToFile(T obj, string filePath, string format = "", bool? encrypt = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _logger.LogError("File path is empty.");
                
                throw new ArgumentNullException(nameof(filePath));
            }
            
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            
            var directory = Path.GetDirectoryName(filePath);
            
            if (!string.IsNullOrEmpty(directory)) 
            {
                Directory.CreateDirectory(directory); // create directory if it doesn't exist'
            }
            
            var myFormat = !string.IsNullOrEmpty(format) ? format : _serializerOptions.DefaultFormat ?? "Compact";
    
            var useEncryption = encrypt ?? _serializerOptions.EnableEncryption;
            
            /*if (File.Exists(filePath))
            {
                _logger.LogInformation($"File already exists: {filePath}");
                throw new ArgumentException($"File already exists: {filePath}");
            }*/
            
            try
            {
                var serializedData = _jsonSerializerWithHooks.Serialize(obj, useEncryption, myFormat);

                File.WriteAllText(filePath, serializedData);
                
                _logger.LogInformation($"Data serialized and saved to file: {filePath}");

                BaseResponse response = new BaseResponse()
                {
                    IsSuccess = true,
                    Messages = new List<Message>()
                    {
                        new Message($"Data serialized and saved to file: {filePath}", Message.MessageLevel.Info)
                    }
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during serialization: {ex.Message}");
                throw;
            }
        }

        public T DeserializeFromFile(string filePath, bool encrypt = false)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _logger.LogError("File path is empty.");
                
                throw new ArgumentNullException(nameof(filePath));
            }
            
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
                var deserializedObject = _jsonSerializerWithHooks.Deserialize(serializedData, encrypt);

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