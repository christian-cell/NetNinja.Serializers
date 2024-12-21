using Moq;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Helpers;

namespace NetNinja.Serializers.Tests.Helpers
{
    public class EncryptionHelperTests
    {
        [Fact]
        public void Encrypt_ShouldEncryptPlainTextSuccessfully()
        {
            var configuration = new EncryptionConfiguration();
            var encryptionHelper = new EncryptionHelper(configuration);
            var plainText = "HelloWorld";

            var encryptedText = encryptionHelper.Encrypt(plainText);

            Assert.NotNull(encryptedText);
            Assert.NotEmpty(encryptedText);

            Assert.NotEqual(plainText, encryptedText);
        }

        [Fact]
        public void Encrypt_ShouldThrowException_WhenEncryptionKeyIsNotSet()
        {
            var configuration = new EncryptionConfiguration
            {
                EncryptionKey = null 
            };

            var encryptionHelper = new EncryptionHelper(configuration);
            var plainText = "HelloWorld";

            var exception = Assert.Throws<InvalidOperationException>(() => encryptionHelper.Encrypt(plainText));
            Assert.Equal("Encryption key is not set.", exception.Message);
        }

        [Fact]
        public void Encrypt_ShouldThrowException_WhenEncryptingNullPlainText()
        {
            var configuration = new EncryptionConfiguration(); 
            var encryptionHelper = new EncryptionHelper(configuration);

            Assert.Throws<ArgumentNullException>(() => encryptionHelper.Encrypt(null));
        }

        [Fact]
        public void Encrypt_ShouldKeepEncryptionConsistent_WithSameKey()
        {
            var configuration = new EncryptionConfiguration();
            var encryptionHelper = new EncryptionHelper(configuration);
            var plainText = "ConsistentText";

            var firstEncryption = encryptionHelper.Encrypt(plainText);
            var secondEncryption = encryptionHelper.Encrypt(plainText);

            Assert.NotNull(firstEncryption);
            Assert.NotNull(secondEncryption);
            Assert.Equal(firstEncryption, secondEncryption); 
        }
    }
};