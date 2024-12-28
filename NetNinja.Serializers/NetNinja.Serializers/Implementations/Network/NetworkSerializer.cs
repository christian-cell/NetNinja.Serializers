using System.Net.Sockets;
using System.Text;
using NetNinja.Serializers.Implementations.ForHooks;

namespace NetNinja.Serializers.Implementations.Network
{
    public class NetworkSerializer<T>
    {
        private readonly JsonSerializerWithHooks<T> _jsonSerializer;

        public NetworkSerializer(JsonSerializerWithHooks<T> jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public void SerializeToNetwork(T obj, string host, int port)
        {
            var serializedData = _jsonSerializer.Serialize(obj, false,"Compact");
            using (var client = new TcpClient(host, port))
            {
                using (var networkStream = client.GetStream())
                {
                    var data = Encoding.UTF8.GetBytes(serializedData);
                    
                    networkStream.Write(data, 0, data.Length);
                    
                    Console.WriteLine("Data sent to network.");
                }
            }
        }

        public T DeserializeFromNetwork(string host, int port)
        {
            using (var client = new TcpClient(host, port))
            {
                using (var networkStream = client.GetStream())
                {
                    var buffer = new byte[1024];
                    
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    var serializedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Console.WriteLine("Data received from network.");
                   
                    return _jsonSerializer.Deserialize(serializedData);
                }
            }
        }
    }
};