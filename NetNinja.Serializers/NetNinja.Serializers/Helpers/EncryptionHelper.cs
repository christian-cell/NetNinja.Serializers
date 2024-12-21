using System.Security.Cryptography;
using System.Text;
using NetNinja.Serializers.Configurations;

namespace NetNinja.Serializers.Helpers
{
    public class EncryptionHelper
    {
        private readonly EncryptionConfiguration _configuration;

        public EncryptionHelper(EncryptionConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Encrypt(string plainText)
        {
            if( plainText == null ) throw new ArgumentNullException(nameof(plainText), "The plain text cannot be null.");
            
            var encryptionKey = _configuration.EncryptionKey ?? throw new InvalidOperationException("Encryption key is not set.");
            
            using (var aes = Aes.Create())
            {
                var key = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32).Substring(0, 32));
                
                aes.Key = key;
                
                aes.IV = Encoding.UTF8.GetBytes(encryptionKey.PadRight(16).Substring(0, 16));

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                  
                    using (var writer = new StreamWriter(cryptoStream))
                    {
                        writer.Write(plainText);
                    }
                    
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public string Decrypt(string encryptedText)
        {
            var encryptionKey = _configuration.EncryptionKey ?? throw new InvalidOperationException("Encryption key is not set.");
            
            using (var aes = Aes.Create())
            {
                var key = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32).Substring(0, 32));
                
                aes.Key = key;
                
                aes.IV = Encoding.UTF8.GetBytes(encryptionKey.PadRight(16).Substring(0, 16));

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
               
                using (var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedText)))
               
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
               
                using (var reader = new StreamReader(cryptoStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
};