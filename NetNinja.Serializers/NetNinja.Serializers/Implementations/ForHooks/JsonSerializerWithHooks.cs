using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NetNinja.Serializers.Helpers;
using Newtonsoft.Json;

namespace NetNinja.Serializers.Implementations.ForHooks
{
    public class JsonSerializerWithHooks<T> : SerializerWithHooks<T>
    {
        private readonly JsonSerializerSettings? _settings;
        private readonly ILogger<JsonSerializerWithHooks<T>> _logger; 
        private readonly EncryptionHelper _encryptionHelper;
        private readonly bool _enableEncryption;

        public JsonSerializerWithHooks(
            EncryptionHelper encryptionHelper,
            ILogger<JsonSerializerWithHooks<T>> logger,
            JsonSerializerSettings? settings = null,
            bool enableEncryption = false
            ) : base()
        {
            _encryptionHelper = encryptionHelper;
            _logger = logger;
            _settings = settings;
            _enableEncryption = enableEncryption;
        }

        #region Sync Methods
        public override string Serialize(T obj, string format = "Compact")
        {
            _logger.LogInformation("Serialization started for type: {Type}", typeof(T).Name);

            var stopwatch = Stopwatch.StartNew(); 
            
            var formatting = format.Equals("Indented", StringComparison.OrdinalIgnoreCase)
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

            if (_enableEncryption)
            {
                serializedData = _encryptionHelper.Encrypt(serializedData);
            }

            stopwatch.Stop();
            
            _logger.LogInformation("Serialization completed in {ElapsedMilliseconds}ms for type: {Type}", stopwatch.ElapsedMilliseconds, typeof(T).Name);
            
            _logger.LogInformation("Serialized data size: {Size} bytes", serializedData.Length);

            return serializedData;
        }

        public override T Deserialize(string data)
        {
            _logger.LogInformation("Deserialization started for type: {Type}", typeof(T).Name);

            var stopwatch = Stopwatch.StartNew();

            var decryptedData = _enableEncryption ? _encryptionHelper.Decrypt(data) : data;

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

        public override async Task<string> SerializeAsync(T obj, string format = "Compact")
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

