using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Helpers;
using Newtonsoft.Json;

namespace NetNinja.Serializers.Implementations.ForHooks
{
    public class JsonSerializerWithHooks<T> : SerializerWithHooks<T>
    {
        private SerializerOptions _serializerOptions;
        private readonly JsonSerializerSettings? _settings;
        private readonly ILogger<JsonSerializerWithHooks<T>> _logger; 
        private readonly EncryptionHelper _encryptionHelper;

        public JsonSerializerWithHooks(
            
            EncryptionHelper encryptionHelper,
            ILogger<JsonSerializerWithHooks<T>> logger, 
            SerializerOptions serializerOptions, 
            JsonSerializerSettings? settings = null
            ) : base()
        {
            _encryptionHelper = encryptionHelper;
            _logger = logger;
            _serializerOptions = serializerOptions;
            _settings = settings;
        }

        #region Sync Methods
        public override string Serialize(T obj, bool? encrypt = null ,string format = "")// Tested
        {
            _logger.LogInformation("Serialization started for type: {Type}", typeof(T).Name);
            
            if (obj == null) throw new ArgumentNullException(nameof(obj), "The object to serialize cannot be null.");
            
            var myFormat = !string.IsNullOrEmpty(format) ? format : _serializerOptions.DefaultFormat ?? "Compact";
    
            var useEncryption = encrypt ?? _serializerOptions.EnableEncryption;

            var stopwatch = Stopwatch.StartNew(); 
            
            var formatting = myFormat.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                ? Formatting.Indented
                : Formatting.None;

            if (BeforeSerialize != null)
            {
                var transformedObj = BeforeSerialize.Invoke(obj);
                
                if (transformedObj is not null)
                {
                    obj = transformedObj;
                }
            }

            var serializedData = JsonConvert.SerializeObject(obj, formatting);

            if (useEncryption)
            {
                serializedData = _encryptionHelper.Encrypt(serializedData);
            }

            stopwatch.Stop();
            
            _logger.LogInformation("Serialization completed in {ElapsedMilliseconds}ms for type: {Type}", stopwatch.ElapsedMilliseconds, typeof(T).Name);
            
            _logger.LogInformation("Serialized data size: {Size} bytes", serializedData.Length);

            return serializedData;
        }

        public override T Deserialize(string data, bool encrypt = false)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data), "The data to deserialize cannot be null or empty.");
            
            _logger.LogInformation("Deserialization started for type: {Type}", typeof(T).Name);

            var stopwatch = Stopwatch.StartNew();
            
            
            var decryptedData = encrypt ? _encryptionHelper.Decrypt(data) : data;

            var obj = JsonConvert.DeserializeObject<T>(decryptedData);

            stopwatch.Stop();
            
            _logger.LogInformation("Deserialization completed in {ElapsedMilliseconds}ms for type: {Type}", stopwatch.ElapsedMilliseconds, typeof(T).Name);
            
            _logger.LogInformation("Deserialized data size: {Size} bytes", decryptedData.Length);

            if (obj != null && AfterDeserialize != null)
            {
                var transformedObj = AfterDeserialize.Invoke(obj);
              
                if (transformedObj is not null)
                {
                    obj = transformedObj;
                }
            }

            return obj;
        }
        #endregion

        #region Async Methods

        public override async Task<string> SerializeAsync(T obj, bool? encrypt = false, string format = "")
        {
            return await Task.Run(() =>
            {
                if (BeforeSerialize != null)
                {
                    var transformedObj = BeforeSerialize.Invoke(obj);
                    if (transformedObj is not null)
                    {
                        obj = transformedObj;
                    }
                }

                var formatting = format.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                    ? Formatting.Indented
                    : Formatting.None;

                return JsonConvert.SerializeObject(obj, formatting, _settings);
            });
        }

        #endregion
    }
};

