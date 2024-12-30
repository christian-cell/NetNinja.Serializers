using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NetNinja.Serializers.Implementations.ForHooks
{
    public class XmlSerializerWithHooks<T> : SerializerWithHooks<T> where T : class
    {
        public override string Serialize(T obj,bool? encrypt = false, string format = "")
        {
            // Aplicar hook de transformación previo (si existe)
            obj = BeforeSerialize?.Invoke(obj) ?? obj;

            // Configuración del escritor de XML
            var xmlSettings = new XmlWriterSettings
            {
                Indent = format.Equals("Indented", StringComparison.OrdinalIgnoreCase),
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };

            // Generar el XML con el formato adecuado
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, xmlSettings))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(xmlWriter, obj);
                }
                return stringWriter.ToString();
            }
        }

        public override T Deserialize(string data, bool encrypt = false)
        {
            using (var stringReader = new StringReader(data))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                var obj = serializer.Deserialize(stringReader) as T;

                if (obj == null)
                    throw new InvalidOperationException("the deserialized object is null..");

                return AfterDeserialize?.Invoke(obj) ?? obj;
            }
        }
    }
};