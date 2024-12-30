using Microsoft.Extensions.Logging;
using Moq;
using NetNinja.Serializers.Configurations;
using NetNinja.Serializers.Implementations.ForHooks;
using NetNinja.Serializers.Implementations.WithDestinations;
using NetNinja.Serializers.Tests.Mocks;
using Xunit.Abstractions;

namespace NetNinja.Serializers.Tests.Implementations.Regular
{
    public class SerializerWithDestinationsTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<ILogger<SerializerWithDestinations<PersonMockVersioned>>> _loggerMock;
        private readonly Mock<ILogger<JsonSerializerWithHooks<PersonMockVersioned>>> _loggerWithHooksrMock;

        public SerializerWithDestinationsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _loggerWithHooksrMock = new Mock<ILogger<JsonSerializerWithHooks<PersonMockVersioned>>>();
            _loggerMock = new Mock<ILogger<SerializerWithDestinations<PersonMockVersioned>>>();
        }
        
        #region SerializeToFile
        [Fact]
        public void SerializeToFile_WithDestinations_Should_return_argumentNullException_if_obj_isnot_provided()
        {
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
                DefaultFormat = "Compact"
            };

            var serializerWithDestinations = new SerializerWithDestinations<PersonMockVersioned>(
                _loggerMock.Object,
                _loggerWithHooksrMock.Object,
                serializerOptions
            );
            
            var filePath = "C:\\Users\\ChristianGarciaMarti\\netNinja\\Serializers\\NetNinja.Serializers.Tests\\Files\\testfile.txt";
            
            var exception = Assert.Throws<ArgumentNullException>(() => serializerWithDestinations.SerializeToFile(null, filePath , "Indented", false));
            
            Assert.Equal("obj", exception.ParamName);
        }
       
        [Fact]
        public void SerializeToFile_WithDestinations_Should_return_argumentNullException_if_filePath_isnot_provided()
        {
            
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
                DefaultFormat = "Compact"
            };

            var serializerWithDestinations = new SerializerWithDestinations<PersonMockVersioned>(
                _loggerMock.Object,
                _loggerWithHooksrMock.Object,
                serializerOptions
            );
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };
            
            var exception = Assert.Throws<ArgumentNullException>(() => serializerWithDestinations.SerializeToFile(person, null , "Indented", false));
            
            Assert.Equal("filePath", exception.ParamName);
        }
        
        [Fact]
        public void SerializeToFile_WithDestinations_Should_create_the_file_with_proper_content_indented_format()
        {
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
                DefaultFormat = "Indented"
            };

            var serializerWithDestinations = new SerializerWithDestinations<PersonMockVersioned>(
                _loggerMock.Object,
                _loggerWithHooksrMock.Object,
                serializerOptions
            );

            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = new[] { "C#", "Azure" }
            };

            var tempDir = Path.GetTempPath(); 
            
            var filePath = Path.Combine(tempDir, "testfile.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var result = serializerWithDestinations.SerializeToFile(person, filePath, "Indented", false);

            Assert.True(result.IsSuccess, "El proceso de serialización falló cuando debía ser exitoso.");

            Assert.True(File.Exists(filePath), "El archivo no fue creado en la ruta especificada.");

            var fileContent = File.ReadAllText(filePath);
            
            _testOutputHelper.WriteLine(fileContent);

            Assert.Contains("\"Name\": \"Ferando\"", fileContent, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("\"Age\": 32", fileContent, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("C#", fileContent, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Azure", fileContent, StringComparison.OrdinalIgnoreCase);

            _testOutputHelper.WriteLine(result.IsSuccess.ToString());
            foreach (var message in result.Messages)
            {
                _testOutputHelper.WriteLine(message.Body);
                _testOutputHelper.WriteLine(message.Level.ToString());
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        
        [Fact]
        public void SerializeToFile_WithDestinations_Should_create_the_file_with_proper_content_compact_format()
        {
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
                DefaultFormat = "Indented"
            };

            var serializerWithDestinations = new SerializerWithDestinations<PersonMockVersioned>(
                _loggerMock.Object,
                _loggerWithHooksrMock.Object,
                serializerOptions
            );

            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = new[] { "C#", "Azure" }
            };

            var tempDir = Path.GetTempPath(); 
            
            var filePath = Path.Combine(tempDir, "testfile.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var result = serializerWithDestinations.SerializeToFile(person, filePath, "Compact", false);

            Assert.True(result.IsSuccess, "El proceso de serialización falló cuando debía ser exitoso.");

            Assert.True(File.Exists(filePath), "El archivo no fue creado en la ruta especificada.");

            var fileContent = File.ReadAllText(filePath);
            
            _testOutputHelper.WriteLine(fileContent);

            Assert.Contains("\"Name\":\"Ferando\"", fileContent);
            Assert.Contains("\"Age\":32", fileContent);
            Assert.Contains("\"Skills\":[\"C#\",\"Azure\"]", fileContent);

            _testOutputHelper.WriteLine(result.IsSuccess.ToString());
            foreach (var message in result.Messages)
            {
                _testOutputHelper.WriteLine(message.Body);
                _testOutputHelper.WriteLine(message.Level.ToString());
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        
        [Fact]
        public void SerializeToFile_Should_Create_Directory_If_Not_Exist()
        {
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
                DefaultFormat = "Compact"
            };

            var serializerWithDestinations = new SerializerWithDestinations<PersonMockVersioned>(
                _loggerMock.Object,
                _loggerWithHooksrMock.Object,
                serializerOptions
            );

            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = new[] { "C#", "Azure" }
            };

            var tempDir = Path.Combine(Path.GetTempPath(), "NonExistentFolder");
            var filePath = Path.Combine(tempDir, "testfile.json");

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            var result = serializerWithDestinations.SerializeToFile(person, filePath, "Indented", false);

            Assert.True(File.Exists(filePath), "El archivo no fue creado correctamente.");

            Assert.True(Directory.Exists(tempDir), "La carpeta no fue creada correctamente.");

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
        #endregion

        #region DeserializeFromFile

        [Fact]
        public void DeserializeFromFile_WithDestinations_Should_return_argumentNullException_if_filePath_is_not_provided()
        {
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
                DefaultFormat = "Compact"
            };

            var serializerWithDestinations = new SerializerWithDestinations<PersonMockVersioned>(
                _loggerMock.Object,
                _loggerWithHooksrMock.Object,
                serializerOptions
            );
            
            var exception = Assert.Throws<ArgumentNullException>(() => serializerWithDestinations.DeserializeFromFile(""));
            
            Assert.Equal("filePath", exception.ParamName);
        }
        
        [Fact]
        public void DeserializeFromFile_WithDestinations_Should_return_FileNotFoundException()
        {
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
                DefaultFormat = "Compact"
            };

            var serializerWithDestinations = new SerializerWithDestinations<PersonMockVersioned>(
                _loggerMock.Object,
                _loggerWithHooksrMock.Object,
                serializerOptions
            );

            var filePath = "C:\\Users\\ChristianGarciaMarti\\netNinja\\Serializers\\NetNinja.Serializers.Tests\\Files\\testfile.jsonasdf";
            
            var exception = Assert.Throws<FileNotFoundException>(() => serializerWithDestinations.DeserializeFromFile(filePath));

            Assert.Contains($"File not found: {filePath}", exception.Message);
            Assert.Contains(filePath, exception.Message);
        }
        
        [Fact]
        public void DeserializeFromFile_WithDestinations_Should_return_proper_T_object()
        {
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
                DefaultFormat = "Compact"
            };

            var serializerWithDestinations = new SerializerWithDestinations<PersonMockVersioned>(
                _loggerMock.Object,
                _loggerWithHooksrMock.Object,
                serializerOptions
            );
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            var filePath = "C:\\Users\\ChristianGarciaMarti\\netNinja\\Serializers\\NetNinja.Serializers.Tests\\Files\\testfile4.txt";
            
            var result = serializerWithDestinations.SerializeToFile(person, filePath, "Compact", false);
            
            _testOutputHelper.WriteLine(result.Messages[0].Body);
            _testOutputHelper.WriteLine(result.IsSuccess.ToString());
            
            if( result.IsSuccess )
            {
                _testOutputHelper.WriteLine(result.Messages[0].Body);
                
                var deserializedPerson = serializerWithDestinations.DeserializeFromFile(filePath);
                
                Assert.Equal(person.Name, deserializedPerson.Name);
                Assert.Equal(person.Age, deserializedPerson.Age);
                Assert.Equal(person.Skills, deserializedPerson.Skills);
            }
            
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }
        }
        
        [Fact]
        public void DeserializeFromFile_WithDestinations_Should_return_proper_T_object_from_encrypted_data()
        {
            SerializerOptions serializerOptions = new SerializerOptions()
            {
                EncryptionConfiguration = new EncryptionConfiguration()
                {
                    EncryptionKey = "defaultKey123"
                },
                EnableEncryption = true,
                DefaultFormat = "Compact"
            };

            var serializerWithDestinations = new SerializerWithDestinations<PersonMockVersioned>(
                _loggerMock.Object,
                _loggerWithHooksrMock.Object,
                serializerOptions
            );
            
            var person = new PersonMockVersioned()
            {
                Name = "Ferando",
                Age = 32,
                Skills = ["C#", "Azure"]
            };

            var filePath = "C:\\Users\\ChristianGarciaMarti\\netNinja\\Serializers\\NetNinja.Serializers.Tests\\Files\\testfile4.txt";
            
            var result = serializerWithDestinations.SerializeToFile(person, filePath, "Compact", true);
            
            _testOutputHelper.WriteLine(result.Messages[0].Body);
            _testOutputHelper.WriteLine(result.IsSuccess.ToString());
            
            if( result.IsSuccess )
            {
                _testOutputHelper.WriteLine(result.Messages[0].Body);
                
                var deserializedPerson = serializerWithDestinations.DeserializeFromFile(filePath, true);
                
                Assert.Equal(person.Name, deserializedPerson.Name);
                Assert.Equal(person.Age, deserializedPerson.Age);
                Assert.Equal(person.Skills, deserializedPerson.Skills);
            }
            
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }
        }

        #endregion
        
    }
};

