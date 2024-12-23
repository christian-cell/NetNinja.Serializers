using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Moq;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Helpers;
using NetNinja.Serializers.Implementations.Regular;
using NetNinja.Serializers.Tests.Mocks;
using Xunit.Abstractions;

namespace NetNinja.Serializers.Tests.Implementations.Regular
{
    public class SystemTextSerializerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly SerializerOptions _serializerOptions = new SerializerOptions()
        {
            EncryptionConfiguration = new EncryptionConfiguration()
            {
                EncryptionKey = "DefaultEncryptionKey123!"
            },
            NewtonSoftJsonSettings = null,
            EnableEncryption = true,
            DefaultFormat = "Compact"
        };

        public SystemTextSerializerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        #region Serialize

        [Fact]
        public void SerializeMessages_should_serialize_messages_correctly()
        {
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>();

            var person = new PersonMock()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            // Act
            var result = systemTextSerializer.Serialize(person);
            
            _testOutputHelper.WriteLine(result);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<string>(result);
            Assert.Equal("{\"Name\":\"Ferando\",\"Age\":32,\"Skills\":[\"C#\",\"Azure\"]}", result);
        }
        
        [Fact]
        public void SerializeMessages_with_options_should_serialize_messages_correctly()
        {
            // Arrange
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                AllowTrailingCommas = true,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
    
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(
                options,
                _serializerOptions
            );

            var person = new PersonMock()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            // Act
            var result = systemTextSerializer.Serialize(person);
    
            _testOutputHelper.WriteLine(result);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<string>(result);
            
            var expectedJson = @"{
  ""name"": ""Ferando"",
  ""age"": 32,
  ""skills"": [
    ""C#"",
    ""Azure""
  ]
}";
            // Assert
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public void Serialize_should_return_null_when_object_is_null()
        {
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.Serialize(null));
            
            Assert.Equal("obj", exception.ParamName);
        }

        #endregion
        
        #region Deserialize
        [Fact]
        public void Deserialize_should_return_null_when_jsonStr_is_null()
        {
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.Deserialize(null));
            
            Assert.Equal("jsonStr", exception.ParamName);
        }
        
        [Fact]
        public void Deserialize_should_return_null_when_jsonStr_is_empty_str()
        {
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.Deserialize(""));
            
            Assert.Equal("jsonStr", exception.ParamName);
        }
        
        [Fact]
        public void Deserialize_should_return_PersonMock_object()
        {
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            var person = new PersonMock()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            // Act
            var personStr = systemTextSerializer.Serialize(person);
            
            var result = systemTextSerializer.Deserialize(personStr);
            
            _testOutputHelper.WriteLine($"Deserialize -> Name: {result.Name}, Age: {result.Age}");
            
            Assert.IsType<NetNinja.Serializers.Tests.Mocks.PersonMock>(result);
            Assert.NotNull(result);
            Assert.Equal(person.Name, result.Name);
            Assert.Equal(person.Age, result.Age);
            Assert.Equal(person.Skills, result.Skills);

        }
        #endregion
        
        #region SerializeMessages
        [Fact]
        public void SerializeMessages_should_return_null_when_object_is_null()
        {
            
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.SerializeMessages(null));
            
            // Assert
            Assert.Equal("messages", exception.ParamName);
        }
        
        [Fact]
        public void SerializeMessages_should_return_correctly_string()
        {
            // Arrange
            var persons = new List<PersonMock>();

            var person = new PersonMock()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            persons.Add(person);
            
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>();
            
            // Act
            var result = systemTextSerializer.SerializeMessages(persons);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<string>(result);
            Assert.Equal("[{\"Name\":\"Ferando\",\"Age\":32,\"Skills\":[\"C#\",\"Azure\"]}]", result);
        }
        #endregion
        
        #region SerializeToStream
        [Fact]
        public void SerializeToStream_should_return_argument_null_exception_when_obj_argument_is_not_provided()
        {
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.SerializeToStream(null, null));
            
            // Assert
            Assert.Equal("obj", exception.ParamName);
        }
        
        [Fact]
        public void SerializeToStream_should_return_argument_null_exception_when_stream_argument_is_not_provided()
        {
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
         
            var person = new PersonMock()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.SerializeToStream(person,null));
            
            // Assert
            Assert.Equal("stream", exception.ParamName);
        }
        
        [Fact]
        public void SerializeToStream_should_return_correctly_data_serialized()
        {
            var serializer = new SystemTextSerializer<PersonMock>();

            var person = new PersonMock()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            using (var memoryStream = new MemoryStream())
            {
                serializer.SerializeToStream(person, memoryStream);
                var result = Encoding.UTF8.GetString(memoryStream.ToArray());
                
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.IsType<string>(result);
                Assert.Equal("{\"Name\":\"Ferando\",\"Age\":32,\"Skills\":[\"C#\",\"Azure\"]}", result);
            }
        }
        #endregion

        #region DeserializeFromStream

        [Fact]
        public void DeserializeFromStream_should_return_null_when_stream_is_null()
        {
            
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.DeserializeFromStream(null));
            
            // Assert
            Assert.Equal("stream", exception.ParamName);
        }
        
        [Fact]
        public void DeserializeFromStream_should_return_T_object()
        {
            
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>();
            
            var person = new PersonMock()
            {
                Name = "Fernando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            var jsonString = systemTextSerializer.Serialize(person);

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                var deserializedFromStream = systemTextSerializer.DeserializeFromStream(memoryStream);
                _testOutputHelper.WriteLine($"DeserializeFromStream -> Name: {deserializedFromStream.Name}, Age: {deserializedFromStream.Age}");
                
                // Assert
                Assert.NotNull(deserializedFromStream);
                Assert.IsType<NetNinja.Serializers.Tests.Mocks.PersonMock>(deserializedFromStream);
            }
        }
        #endregion
        
        #region SerializeCompressed
        [Fact]
        public void SerializeCompressed_should_return_null_when_obj_is_null()
        {
            
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.SerializeCompressed(null));
            
            // Assert
            Assert.Equal("obj", exception.ParamName);
        }
        
        [Fact]
        public void SerializeCompressed_should_return_compressed_data_for_valid_object()
        {
            // Arrange
            var systemTextSerializer = new SystemTextSerializer<PersonMock>();
            var testObject = new PersonMock { Name = "John Doe", Age = 30 }; 

            // Act
            var compressedData = systemTextSerializer.SerializeCompressed(testObject);

            // Assert
            Assert.NotNull(compressedData); 
            Assert.NotEmpty(compressedData); 
        }
        
        [Fact]
        public void SerializeCompressed_should_return_same_output_for_identical_objects()
        {
            // Arrange
            var systemTextSerializer = new SystemTextSerializer<PersonMock>();
            var obj1 = new PersonMock { Name = "John Doe", Age = 30 };
            var obj2 = new PersonMock { Name = "John Doe", Age = 30 };

            // Act
            var compressedData1 = systemTextSerializer.SerializeCompressed(obj1);
            var compressedData2 = systemTextSerializer.SerializeCompressed(obj2);

            // Assert
            Assert.Equal(compressedData1, compressedData2);
        }
        
        [Fact]
        public void SerializeCompressed_should_return_different_output_for_different_objects()
        {
            // Arrange
            var systemTextSerializer = new SystemTextSerializer<PersonMock>();
            var obj1 = new PersonMock { Name = "John Doe", Age = 30 };
            var obj2 = new PersonMock { Name = "Jane Doe", Age = 25 };

            // Act
            var compressedData1 = systemTextSerializer.SerializeCompressed(obj1);
            var compressedData2 = systemTextSerializer.SerializeCompressed(obj2);

            // Assert
            Assert.NotEqual(compressedData1, compressedData2);
        }
        
        [Fact]
        public void SerializeCompressed_should_allow_decompression_and_match_original_serialized_data()
        {
            // Arrange
            var systemTextSerializer = new SystemTextSerializer<PersonMock>();
            var testObject = new PersonMock { Name = "John Doe", Age = 30 };

            // Act
            var compressedData = systemTextSerializer.SerializeCompressed(testObject);
            var decompressedData = CompressionHelper.Decompress(compressedData);
            var serializedData = Encoding.UTF8.GetBytes(systemTextSerializer.Serialize(testObject));

            // Assert
            Assert.Equal(serializedData, decompressedData);
        }
        #endregion
        
        #region DeserializeCompressed
        
        [Fact]
        public void DeserializeCompressed_should_return_null_when_compressedData_is_null()
        {
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.DeserializeCompressed(null));
            
            Assert.Equal("compressedData", exception.ParamName);
        }
        
        [Fact]
        public void DeserializeCompressed_should_return_personMock_object()
        {
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            var person = new PersonMock()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            // Act
            var compressedStr = systemTextSerializer.SerializeCompressed(person);

            var result = systemTextSerializer.DeserializeCompressed(compressedStr);
            
            _testOutputHelper.WriteLine($"Deserialize -> Name: {result.Name}, Age: {result.Age}");
            
            Assert.IsType<NetNinja.Serializers.Tests.Mocks.PersonMock>(result);
            Assert.NotNull(result);
            Assert.Equal(person.Name, result.Name);
            Assert.Equal(person.Age, result.Age);
            Assert.Equal(person.Skills, result.Skills);
        }
        #endregion

        #region ValidateSerializedData

        [Fact]
        public void ValidateSerializedData_should_return_null_when_schema_is_null()
        {
            
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            var person = new PersonMock()
            {
                Name = "Fernando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            var personStr = systemTextSerializer.Serialize(person);
            
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.ValidateSerializedData(personStr, null));
            
            // Assert
            Assert.Equal("schema", exception.ParamName);
        }
        
        [Fact]
        public void ValidateSerializedData_should_return_null_when_serializedData_is_null()
        {
            
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            const string schema = @"
            {
                'type': 'object',
                'properties': {
                    'Name': { 'type': 'string' },
                    'Age': { 'type': 'integer' },
                    'Skills': {
                        'type': 'array',
                        'items': { 'type': 'string' }
                    }
                },
                'required': ['Name', 'Age']
            }";
            
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => systemTextSerializer.ValidateSerializedData(null, schema));
            
            // Assert
            Assert.Equal("serializedData", exception.ParamName);
        }
        
        [Fact]
        public void ValidateSerializedData_should_return_proper_true()
        {
            
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            var person = new PersonMock()
            {
                Name = "Fernando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            const string schema = @"
            {
                'type': 'object',
                'properties': {
                    'Name': { 'type': 'string' },
                    'Age': { 'type': 'integer' },
                    'Skills': {
                        'type': 'array',
                        'items': { 'type': 'string' }
                    }
                },
                'required': ['Name', 'Age']
            }";
            
            var personStr = systemTextSerializer.Serialize(person);
            
            // Act
            var result = systemTextSerializer.ValidateSerializedData(personStr, schema);
            
            // _testOutputHelper.WriteLine($"ValidateSerializedData -> Result: {result}");
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void ValidateSerializedData_should_return_proper_false()
        {
            
            // Arrange
            SystemTextSerializer<PersonMock> systemTextSerializer = new SystemTextSerializer<PersonMock>(); 
            
            var person = new PersonMock()
            {
                Name = "Fernando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            const string schema = @"
            {
                'type': 'object',
                'properties': {
                    'Name': { 'type': 'string' },
                    'Edad': { 'type': 'integer' },
                    'Skills': {
                        'type': 'array',
                        'items': { 'type': 'string' }
                    }
                },
                'required': ['Name', 'Edad']
            }";
            
            var personStr = systemTextSerializer.Serialize(person);
            
            // Act
            var result = systemTextSerializer.ValidateSerializedData(personStr, schema);
            
            // to print _testOutputHelper.WriteLine($"ValidateSerializedData -> Result: {result}");
            
            // Assert
            Assert.False(result);
        }
        #endregion
    }
};