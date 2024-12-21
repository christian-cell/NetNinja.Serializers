using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Helpers;

namespace NetNinja.Serializers.Implementations.Regular
{
    public class XmlSerializer<T> : IXmlSerializer<T>, ISerializer<T>
    {
        #region Serialization Methods

        public string SerializeToXml(T obj)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            
            using var stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }
        
        public string Serialize(T obj)
        {
            return SerializeToXml(obj); 
        }
        
        public void SerializeToStream(T obj, Stream stream)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using var streamWriter = new StreamWriter(stream);
            xmlSerializer.Serialize(streamWriter, obj);
        }
        
        public string SerializeMessages(List<T> messages)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
            using var stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, messages);
            return stringWriter.ToString();
        }

        #endregion

        #region Deserialization Methods

        public T DeserializeFromXml(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            
            using var stringReader = new StringReader(xml);
            return (T)xmlSerializer.Deserialize(stringReader)!;
        }

        public T Deserialize(string serializedData)
        {
            return DeserializeFromXml(serializedData); 
        }

        public T DeserializeFromStream(Stream stream)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using var streamReader = new StreamReader(stream);
            return (T)xmlSerializer.Deserialize(streamReader)!;
        }

        #endregion

        #region Compressed Methods

        public byte[] SerializeCompressed(T obj)
        {
            var serializedData = Encoding.UTF8.GetBytes(Serialize(obj));
            return CompressionHelper.Compress(serializedData);
        }

        public T DeserializeCompressed(byte[] compressedData)
        {
            var decompressedData = CompressionHelper.Decompress(compressedData);
            var xml = Encoding.UTF8.GetString(decompressedData);
            return Deserialize(xml);
        }

        #endregion

        #region ValidationArea

        public bool ValidateSerializedData(string serializedData, string schema)
        {
            try
            {
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(null, XmlReader.Create(new StringReader(schema)));

                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemaSet
                };

                settings.ValidationEventHandler += (sender, e) =>
                {
                    throw new Exception($"Validation failed: {e.Message}");
                };

                using (StringReader stringReader = new StringReader(serializedData))
                {
                    using (XmlReader reader = XmlReader.Create(stringReader, settings))
                    {
                        while (reader.Read()) ; 
                    }
                }

                return true; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"XML Validation Error: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
};

