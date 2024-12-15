using System.Data;
using System.Text;
using NetNinja.Serializers.Implementations.ForHooks;
using Newtonsoft.Json;

namespace NetNinja.Serializers.Implementations.ForDataSets
{
    public class DataTableSerializerWithHooks : SerializerWithHooks<DataTable>
    {
        #region Sync Methods
        
        public override string Serialize(DataTable dataTable, string format = "Compact")
        {
            if (BeforeSerialize != null)
            {
                dataTable = BeforeSerialize.Invoke(dataTable);
            }
            
            Formatting formatting = format.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                ? Formatting.Indented
                : Formatting.None;

            return JsonConvert.SerializeObject(dataTable, formatting);
        }

        public override DataTable Deserialize(string data)
        {
            var dataTable = JsonConvert.DeserializeObject<DataTable>(data);

            if (AfterDeserialize != null)
            {
                dataTable = AfterDeserialize.Invoke(dataTable!);
            }

            return dataTable!;
        }

        public override string CombineSerialized(IEnumerable<DataTable> objects)
        {
            var serializedObjects = new List<string>();

            foreach (var obj in objects)
            {
                serializedObjects.Add(Serialize(obj));
            }

            return "[" + string.Join(",", serializedObjects) + "]";
        }

        public override IEnumerable<DataTable> SplitSerialized(string combinedSerialized)
        {
            var serializedArray = JsonConvert.DeserializeObject<List<string>>(combinedSerialized) 
                                  ?? new List<string>();

            foreach (var item in serializedArray)
            {
                var obj = Deserialize(item);
                yield return obj;
            }
        }
        
        #endregion

        #region Async Methods

        public override async Task<string> SerializeAsync(DataTable dataTable, string format = "Compact")
        {
            return await Task.Run(() =>
            {
                if (BeforeSerialize != null)
                {
                    dataTable = BeforeSerialize.Invoke(dataTable);
                }

                Formatting formatting = format.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                    ? Formatting.Indented
                    : Formatting.None;

                return JsonConvert.SerializeObject(dataTable, formatting);
            });
        }
        
        public override async Task<DataTable> DeserializeAsync(string data)
        {
            return await Task.Run(() =>
            {
                var dataTable = JsonConvert.DeserializeObject<DataTable>(data);

                if (AfterDeserialize != null)
                {
                    dataTable = AfterDeserialize.Invoke(dataTable);
                }

                return dataTable;
            });
        }
        
        public override async Task<string> CombineSerializedAsync(IEnumerable<DataTable> objects)
        {
            return await Task.Run(() =>
            {
                var serializedObjects = new List<string>();

                foreach (var obj in objects)
                {
                    serializedObjects.Add(Serialize(obj));
                }

                return "[" + string.Join(",", serializedObjects) + "]";
            });
        }
        
        public override async Task<IEnumerable<DataTable>> SplitSerializedAsync(string combinedSerialized)
        {
            return await Task.Run(() =>
            {
                var result = new List<DataTable>();

                var serializedArray = JsonConvert.DeserializeObject<List<string>>(combinedSerialized) ?? new List<string>();

                foreach (var item in serializedArray)
                {
                    var obj = Deserialize(item);
                    result.Add(obj); 
                }

                return (IEnumerable<DataTable>)result; 
            });
        }
        
        public async Task SerializeToFileAsync(DataTable dataTable, string filePath)
        {
            string serializedData = await SerializeAsync(dataTable);

            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(serializedData);
            }
        }

        public async Task<DataTable> DeserializeFromFileAsync(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string data = await reader.ReadToEndAsync();
                return await DeserializeAsync(data);
            }
        }

        #endregion
    }
};

