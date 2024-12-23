using System.Text;
using MongoDB.Bson;
using NetNinja.Serializers.Helpers;
using NetNinja.Serializers.Implementations.Versioned;
using NetNinja.Serializers.Tests.Mocks;
using Xunit.Abstractions;

namespace NetNinja.Serializers.Tests.Implementations.Regular
{
    public class BsonSerializerTests
    {
        
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly BsonSerializerWithVersion<PersonMockVersioned> _sut;

        public BsonSerializerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _sut = new BsonSerializerWithVersion<PersonMockVersioned>();
        }

        #region Serialize

        [Fact]
        public void Serialize_bson_Should_return_argumentNullException_if_obj_isnot_provided()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.Serialize(null));
            
            Assert.Equal("obj", exception.ParamName);
        }
        
        [Fact]
        public void Serialize_bson_Should_return_proper_string()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            var result = bsonSerializerWithVersion.Serialize(person);
            
            // _testOutputHelper.WriteLine(result);
            
            Assert.IsType<string>(result);
            Assert.NotEmpty(result);
            Assert.NotNull(result);
        }
        #endregion

        #region SerializeCompressed
        [Fact]
        public void SerializeCompressed_bson_should_return_compressed_data_for_valid_object()
        {
            var bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>();
            var testObject = new PersonMockVersioned { Name = "John Doe", Age = 30 }; 

            var compressedData = bsonSerializerWithVersion.SerializeCompressed(testObject);

            Assert.NotNull(compressedData); 
            Assert.NotEmpty(compressedData); 
        }
        
        [Fact]
        public void SerializeCompressed_bson_should_return_same_output_for_identical_objects()
        {
            var bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>();
            var obj1 = new PersonMockVersioned { Name = "John Doe", Age = 30 };
            var obj2 = new PersonMockVersioned { Name = "John Doe", Age = 30 };

            var compressedData1 = bsonSerializerWithVersion.SerializeCompressed(obj1);
            var compressedData2 = bsonSerializerWithVersion.SerializeCompressed(obj2);

            Assert.Equal(compressedData1, compressedData2);
        }
        
        [Fact]
        public void SerializeCompressed_bson_should_return_different_output_for_different_objects()
        {
            var bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>();
            var obj1 = new PersonMockVersioned { Name = "John Doe", Age = 30 };
            var obj2 = new PersonMockVersioned { Name = "Jane Doe", Age = 25 };

            var compressedData1 = bsonSerializerWithVersion.SerializeCompressed(obj1);
            var compressedData2 = bsonSerializerWithVersion.SerializeCompressed(obj2);

            Assert.NotEqual(compressedData1, compressedData2);
        }
        
        [Fact]
        public void SerializeCompressed_bson_should_allow_decompression_and_match_original_serialized_data()
        {
            var bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>();
            var testObject = new PersonMockVersioned { Name = "John Doe", Age = 30, Skills = ["C#", "Azure"], Version = "1"};

            var serializedData = bsonSerializerWithVersion.Serialize(testObject);
            var compressedData = CompressionHelper.Compress(Encoding.UTF8.GetBytes(serializedData));
            var decompressedData = CompressionHelper.Decompress(compressedData);

            var deserializedObject = bsonSerializerWithVersion.Deserialize(Encoding.UTF8.GetString(decompressedData));

            Assert.Equal(testObject.Name, deserializedObject.Name);
            Assert.Equal(testObject.Age, deserializedObject.Age);
            Assert.Equal(testObject.Version, deserializedObject.Version);
            Assert.Equal(testObject.Skills, deserializedObject.Skills);
        }

        #endregion
        
        #region SerializeToStream
        [Fact]
        public void SerializeToStream_Should_return_argumentNullException_if_stream_isnot_provided()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exampleObject = new PersonMockVersioned
            {
                Name = "Example User",
                Age = 35,
                Skills = new[] { "C#", "BSON", "Serialization" },
                Version = "1.0"
            };
            
            using (var memoryStream = new MemoryStream())
            {
                
                var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.SerializeToStream(exampleObject, null));
                
                Assert.Equal("stream", exception.ParamName);
            }
        }
        
        [Fact]
        public void SerializeToStream_Should_return_argumentNullException_if_obj_isnot_provided()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
           
            
            using (var memoryStream = new MemoryStream())
            {
                
                var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.SerializeToStream(null, memoryStream));
                
                Assert.Equal("obj", exception.ParamName);
            }
        }
        #endregion
        
        #region SerializeMessages
        [Fact]
        public void Serialize_messages_bson_Should_return_argumentNullException_if_messages_isnot_provided()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.SerializeMessages(null));
            
            Assert.Equal("messages", exception.ParamName);
        }
        
        [Fact]
        public void Serialize_messages_bson_Should_return_proper_string()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var personList = new List<PersonMockVersioned>()
            {
                new PersonMockVersioned() { Name = "Ferando", Age = 32, Skills = ["C#", "Azure"], Version = "1.0" },
                new PersonMockVersioned() { Name = "Maria", Age = 28, Skills = ["Python"], Version = "2.0" }
            };
            
            var serializedList = bsonSerializerWithVersion.SerializeMessages(personList);
            var deserializedList = bsonSerializerWithVersion.DeserializeMessages(serializedList);

            
            // Assert
            Assert.NotNull(deserializedList);
            Assert.Equal(2, deserializedList.Count);
            
            Assert.Equal(personList[0].Name, deserializedList[0].Name);
            Assert.Equal(personList[0].Age, deserializedList[0].Age);
            Assert.Equal(personList[0].Version, deserializedList[0].Version);
            Assert.Equal(personList[0].Skills, deserializedList[0].Skills);
            
            
            Assert.Equal(personList[1].Name, deserializedList[1].Name);
            Assert.Equal(personList[1].Age, deserializedList[1].Age);
            Assert.Equal(personList[1].Version, deserializedList[1].Version);
            Assert.Equal(personList[1].Skills, deserializedList[1].Skills);
        }
        #endregion
        
        #region DeserializeMessages
        [Fact]
        public void DeserializeMessages_bson_Should_return_argumentNullException_if_serializedData_isnot_provided()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.DeserializeMessages(null));
            
            Assert.Equal("serializedData", exception.ParamName);
        }
        
        [Fact]
        public void DeserializeMessages_bson_Should_return_list_of_t()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            var persons = new List<PersonMockVersioned>();

            persons.Add(person);

            var personsStr = bsonSerializerWithVersion.SerializeMessages(persons);
            
            var result = bsonSerializerWithVersion.DeserializeMessages(personsStr);
            
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<List<PersonMockVersioned>>(result);
        }
        #endregion

        #region Deserialize
        [Fact]
        public void Deserialize_bson_Should_return_argumentNullException_if_serializedData_isnot_provided()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.Deserialize(null));
            
            Assert.Equal("serializedData", exception.ParamName);
        }
        
        [Fact]
        public void Deserialize_bson_Should_return_argumentNullException_if_serializedData_is_empty_string()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.Deserialize(""));
            
            Assert.Equal("serializedData", exception.ParamName);
        }
        
        [Fact]
        public void Deserialize_bson_Should_returng_t_object()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            var personStr = bsonSerializerWithVersion.Serialize(person);
            
            var result = bsonSerializerWithVersion.Deserialize(personStr);
            
            Assert.NotNull(result);
            Assert.IsType<PersonMockVersioned>(result);
        }
        #endregion

        #region DeserializeFromStream

        [Fact]
        public void DeserializeFromStream_bson_Should_return_argumentNullException_if_serializedData_is_empty_string()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.DeserializeFromStream(null));
            
            Assert.Equal("stream", exception.ParamName);
        }
        
        [Fact]
        public void DeserializeFromStream_bson_Should_return_t_object()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            using (var memoryStream = new MemoryStream())
            {
                bsonSerializerWithVersion.SerializeToStream(person, memoryStream);
                memoryStream.Position = 0; 
                var result = bsonSerializerWithVersion.DeserializeFromStream(memoryStream);
                
                Assert.NotNull(result);
                Assert.IsType<PersonMockVersioned>(result);
                Assert.Equal(person.Name, result.Name);
                Assert.Equal(person.Age, result.Age);
            }
        }
        #endregion

        #region DeserializeCompressed
        [Fact]
        public void DeserializeCompressed_bson_Should_return_argumentNullException_if_compressedData_is_empty_string()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.DeserializeCompressed(null));
            
            Assert.Equal("compressedData", exception.ParamName);
        }
        
        [Fact]
        public void DeserializeCompressed_bson_Should_return_t_object()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            byte[] compressedData = bsonSerializerWithVersion.SerializeCompressed(person);
            
            var result = bsonSerializerWithVersion.DeserializeCompressed(compressedData);
            
            Assert.NotNull(result);
            Assert.IsType<PersonMockVersioned>(result);
            Assert.Equal(person.Name, result.Name);
            Assert.Equal(person.Age, result.Age);
        }
        #endregion
        
        #region ValidateSerializedData
        
        [Fact]
        public void ValidateSerializedData_bson_Should_return_argumentNullException_if_serializedData_is_empty_string()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.ValidateSerializedData("" , string.Empty));
            
            Assert.Equal("serializedData", exception.ParamName);
        }
        
        [Fact]
        public void ValidateSerializedData_bson_Should_return_true()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            var bsonString = bsonSerializerWithVersion.Serialize(person);
            
            var result = bsonSerializerWithVersion.ValidateSerializedData(bsonString , string.Empty);
            
            _testOutputHelper.WriteLine($"{result}");
            
            Assert.True(result);
        }
        #endregion
        
        #region SerializeWithVersion
        [Fact]
        public void SerializeWithVersion_bson_Should_return_argumentNullException_if_obj_is_null()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.SerializeWithVersion(null , "2"));
            
            Assert.Equal("obj", exception.ParamName);
        }
        
        [Fact]
        public void SerializeWithVersion_bson_Should_return_argumentNullException_if_version_is_null()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"],
                Version = "1.0"
            };
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.SerializeWithVersion(person , null));
            
            Assert.Equal("version", exception.ParamName);
        }
        
        [Fact]
        public void SerializeWithVersion_bson_Should_return_argumentNullException_if_version_is_empty_string()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"],
                Version = "1.0"
            };
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.SerializeWithVersion(person , ""));
            
            Assert.Equal("version", exception.ParamName);
        }
        
        [Fact]
        public void SerializeWithVersion_bson_Should_return_bsondocument()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"],
                Version = "1.0"
            };
            
            var version = "2.0";
            
            var result = bsonSerializerWithVersion.SerializeWithVersion(person , version);
            
            Assert.NotNull(result);
            Assert.IsType<BsonDocument>(result);
            Assert.NotEmpty(result.ToString());
            Assert.True(result.Contains("Version"));
            Assert.True(result.Contains("Name"));
            Assert.True(result.Contains("Age"));
            Assert.True(result.Contains("Skills"));
        }
        #endregion
        
        #region DeserializeWithVersion
        [Fact]
        public void DeserializeWithVersion_bson_Should_return_argumentNullException_if_bsonData_is_null()
        {
            BsonSerializerWithVersion<PersonMockVersioned> bsonSerializerWithVersion = new BsonSerializerWithVersion<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.DeserializeWithVersion(null));
            
            Assert.Equal("bsonData", exception.ParamName);
        }
        
        [Fact]
        public void Should_ThrowArgumentNullException_When_BsonDataIsNull()
        {
            BsonDocument bsonData = null;

            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeserializeWithVersion(bsonData));

            Assert.Equal("bsonData", exception.ParamName); 
        }

        [Fact]
        public void Should_ReturnExpectedObjectAndVersion_When_DataIsValid()
        {
            var bsonDoc = new BsonDocument
            {
                { "Name", "John Doe" },
                { "Age", 30 },
                { "Skills", new BsonArray { "C#", "Azure" } },
                { "Version", "1.0" }
            };

            var expectedPerson = new PersonMockVersioned
            {
                Name = "John Doe",
                Age = 30,
                Skills = new string[] { "C#", "Azure" }
            };

            var (deserializedObject, version) = _sut.DeserializeWithVersion(bsonDoc);

            Assert.NotNull(deserializedObject);
            Assert.Equal(expectedPerson.Name, deserializedObject.Name);
            Assert.Equal(expectedPerson.Age, deserializedObject.Age);
            Assert.Equal(expectedPerson.Skills, deserializedObject.Skills);
            Assert.Equal("1.0", version);
        }

        [Fact]
        public void Should_ThrowKeyNotFoundException_When_VersionFieldIsAbsent()
        {
            var bsonDoc = new BsonDocument
            {
                { "Name", "John Doe" },
                { "Age", 30 },
                { "Skills", new BsonArray { "C#", "Azure" } }
            };

            var exception = Assert.Throws<KeyNotFoundException>(() => _sut.DeserializeWithVersion(bsonDoc));

            Assert.Contains("Version", exception.Message); 
        }

        [Fact]
        public void Should_HandleEmptyDocument_When_BsonDataHasNoFields()
        {
            var bsonDoc = new BsonDocument();

            var exception = Assert.Throws<KeyNotFoundException>(() => _sut.DeserializeWithVersion(bsonDoc));

            Assert.Contains("Version", exception.Message);
        }
        #endregion
    }
};

