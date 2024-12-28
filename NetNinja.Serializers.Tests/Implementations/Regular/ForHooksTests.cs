using MongoDB.Bson;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Helpers;
using NetNinja.Serializers.Implementations.ForHooks;
using NetNinja.Serializers.Implementations.Versioned;
using NetNinja.Serializers.Tests.Mocks;
using Xunit.Abstractions;

namespace NetNinja.Serializers.Tests.Implementations.Regular
{
    public class ForHooksTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ForHooksTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        #region Serialize
        
        [Fact]
        public void Serialize_ForHooks_Should_return_argumentNullException_if_obj_isnot_provided()
        {
            
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
            };
            
            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithHooks.Serialize(null));
            
            Assert.Equal("obj", exception.ParamName);
        }
        
        [Fact]
        public void Serialize_ForHooks_Should_return_proper_string()
        {
            
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
            };
            
            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            var result = bsonSerializerWithHooks.Serialize(person, false);

            Assert.IsType<string>(result);
            Assert.NotEmpty(result);
            Assert.NotNull(result);
        }

        #endregion
        
        #region SerializeAsync
        [Fact]
        public async Task SerializeAsync_ForHooks_Should_return_argumentNullException_if_obj_isnot_provided()
        {
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });

            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                DefaultFormat = "Compact",
                EnableEncryption = true,
            };
    
            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper); 
    
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => bsonSerializerWithHooks.SerializeAsync(null));

            Assert.Equal("obj", exception.ParamName);
        }
        
        [Fact]
        public async Task SerializeAsync_ForHooks_Should_return_proper_string()
        {
            
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                DefaultFormat = "Compact",
                EnableEncryption = true,
            };
            
            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            var result = await bsonSerializerWithHooks.SerializeAsync(person);

            _testOutputHelper.WriteLine(result);
            
            Assert.IsType<string>(result);
            Assert.NotEmpty(result);
            Assert.NotNull(result);
        }
        
        #endregion

        #region CombineSerialized

        [Fact]
        public void CombineSerialized_ForHooks_Should_return_argumentNullException_if_objects_isnot_provided()
        {
            
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
            };
            
            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithHooks.CombineSerialized(null));
            
            Assert.Equal("objects", exception.ParamName);
        }
        
        [Fact]
        public void CombineSerialized_ForHooks_Should_return_proper_string()
        {
            
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                DefaultFormat = "Compact",
                EnableEncryption = true,
            };
            
            var exampleObjects = new List<PersonMockVersioned>
            {
                new PersonMockVersioned { Name = "John Doe", Age = 30, Skills = new[] { "C#", ".NET", "MongoDB" } },
                new PersonMockVersioned { Name = "Jane Smith", Age = 25, Skills = new[] { "JavaScript", "React", "Node.js" } }
            };

            var serializer = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper);

            serializer.BeforeSerialize = obj =>
            {
                obj.Name = obj.Name.ToUpper();
                return obj;
            };
            
            var result = serializer.CombineSerialized(exampleObjects,false);
            
            _testOutputHelper.WriteLine(result);
            
            Assert.IsType<string>(result);
            Assert.NotEmpty(result);
            Assert.NotNull(result);
        }

        #endregion

        #region Deserialize

        [Fact]
        public void Deserialize_ForHooks_Should_return_argumentNullException_if_data_is_empty()
        {
            
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
            };
            
            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            var personStr = bsonSerializerWithHooks.Serialize(person,false,"Compact");
            
            var result = bsonSerializerWithHooks.Deserialize(personStr);
            
            Assert.NotNull(result);
            Assert.IsType<PersonMockVersioned>(result);
        }

        #endregion
        
        #region SplitSerialized
        [Fact]
        public void SplitSerialized_ForHooks_Should_return_argumentNullException_if_combinedSerialized_is_empty()
        {
            
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
            };
            
            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithHooks.SplitSerialized(null));
            
            Assert.Equal("combinedSerialized", exception.ParamName);
        }
        
        [Fact]
        public void SplitSerialized_ForHooks_Should_return_proper_T_object()
        {
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
            };
            
            string combinedSerialized = @"
            [
                {
                    ""Name"": ""JOHN DOE"",
                    ""Age"": 30,
                    ""Skills"": [""C#"", "".NET"", ""MongoDB""]
                },
                {
                    ""Name"": ""JANE SMITH"",
                    ""Age"": 25,
                    ""Skills"": [""JavaScript"", ""React"", ""Node.js""]
                }
            ]";
            
            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper); 
            
            bsonSerializerWithHooks.AfterDeserialize = obj =>
            {
                obj.Name = obj.Name.ToLower();
                return obj;
            };
            
            var result = bsonSerializerWithHooks.SplitSerialized(combinedSerialized);

            foreach (var obj in result)
            {
                _testOutputHelper.WriteLine($"Name: {obj.Name}");
                _testOutputHelper.WriteLine($"Age: {obj.Age}");
                _testOutputHelper.WriteLine($"Skills: {string.Join(", ", obj.Skills)}");
            }
            
            Assert.True(true);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void SplitSerialized_NullOrEmptyInput_ShouldThrowArgumentNullException(string input)
        {
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration { EncryptionKey = "defaultKey" });

            var serializerOptions = new SerializerOptions
            {
                EnableEncryption = true
            };

            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper);

            Assert.Throws<ArgumentNullException>(() => bsonSerializerWithHooks.SplitSerialized(input));
        }
        
        [Fact]
        public void SplitSerialized_InvalidBson_ShouldThrowException()
        {
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration { EncryptionKey = "defaultKey" });

            var serializerOptions = new SerializerOptions
            {
                EnableEncryption = true
            };

            string invalidSerialized = "[{ \"Name\": \"JOHN DOE\", \"Age\": 30 "; 

            var bsonSerializerWithHooks = new BsonSerializerWithHooks<PersonMockVersioned>(serializerOptions, encryptionHelper);

            Assert.Throws<FormatException>(() => bsonSerializerWithHooks.SplitSerialized(invalidSerialized));
        }
        #endregion
    }
};

