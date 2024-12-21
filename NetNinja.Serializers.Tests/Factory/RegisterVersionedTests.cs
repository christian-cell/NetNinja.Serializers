namespace NetNinja.Serializers.Tests.Factory
{
    public class RegisterVersionedTests
    {
        public static readonly Dictionary<string, Func<Type, object>> VersionedSerializers = new();

        public RegisterVersionedTests()
        {
            VersionedSerializers.Clear();
        }

        public static void RegisterVersioned(string format, Func<Type, object> serializerFactory)
        {
            var normalizedFormat = format.ToUpper();

            if (VersionedSerializers.ContainsKey(normalizedFormat))
                throw new InvalidOperationException($"Versioned serializer for '{format}' is already registered.");

            VersionedSerializers[normalizedFormat] = serializerFactory;
        }

        [Fact]
        public void Register_NewFormat_Success()
        {
            var format = "JSON";
            Func<Type, object> serializerFactory = (type) => new object();

            RegisterVersioned(format, serializerFactory);

            Assert.True(VersionedSerializers.ContainsKey(format.ToUpper()));
            Assert.Equal(serializerFactory, VersionedSerializers[format.ToUpper()]);
        }

        [Fact]
        public void Register_DuplicateFormat_ThrowsInvalidOperationException()
        {
            var format = "JSON";
            Func<Type, object> serializerFactory = (type) => new object();

            RegisterVersioned(format, serializerFactory);

            var exception = Assert.Throws<InvalidOperationException>(() => RegisterVersioned(format, serializerFactory));
            Assert.Equal($"Versioned serializer for '{format}' is already registered.", exception.Message);
        }

        [Fact]
        public void Register_FormatInsensitiveToCase_ThrowsInvalidOperationException()
        {
            var format = "JSON";
            var formatLowerCase = "json";
            Func<Type, object> serializerFactory = (type) => new object();

            RegisterVersioned(format, serializerFactory);

            var exception = Assert.Throws<InvalidOperationException>(() => RegisterVersioned(formatLowerCase, serializerFactory));
            Assert.Equal($"Versioned serializer for '{formatLowerCase}' is already registered.", exception.Message);
        }

        [Fact]
        public void Register_ValidateSerializerFactory_Success()
        {
            var format = "XML";
            bool factoryInvoked = false;

            Func<Type, object> serializerFactory = (type) =>
            {
                factoryInvoked = true;
                return new object();
            };

            RegisterVersioned(format, serializerFactory);

            var serializer = VersionedSerializers[format.ToUpper()](typeof(string));

            Assert.True(factoryInvoked);
            Assert.NotNull(serializer);
        }
    }
}

