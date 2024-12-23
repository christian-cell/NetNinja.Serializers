using Newtonsoft.Json;

namespace NetNinja.Serializers.Configurations
{
    public class SerializerOptions
    {
        public EncryptionConfiguration EncryptionConfiguration { get; set; } = new EncryptionConfiguration();
        public JsonSerializerSettings NewtonSoftJsonSettings { get; set; } = new JsonSerializerSettings { Formatting = Formatting.None };
        public bool EnableEncryption { get; set; } = false; 
        public string DefaultFormat { get; set; } = "Compact"; 
    }
};

