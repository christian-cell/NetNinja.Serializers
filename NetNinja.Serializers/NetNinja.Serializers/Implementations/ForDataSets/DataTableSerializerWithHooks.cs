using System.Data;
using System.Text;
using NetNinja.Serializers.Implementations.ForHooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static IEnumerable<DataTable> SplitSerialized(string combinedSerialized)
        {
            var serializedArray = JsonConvert.DeserializeObject<List<string>>(combinedSerialized) ?? new List<string>();

            foreach (var item in serializedArray)
            {
                var obj = DeserializeFromSplitted(item);
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
        
        public static async Task<IEnumerable<DataTable>> SplitSerializedAsync(string combinedSerialized)
        {
            return await Task.Run(() =>
            {
                var result = new List<DataTable>();

                var serializedArray = JsonConvert.DeserializeObject<List<string>>(combinedSerialized) ?? new List<string>();

                foreach (var item in serializedArray)
                {
                    var dataTable = BuildDataTable(item);
                    result.Add(dataTable);
                }

                return result;
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

        #region private methods

        private static DataTable DeserializeFromSplitted(string serializedDataTable)
        {
            var tableObject = JsonConvert.DeserializeObject<JObject>(serializedDataTable);

            var dataTable = new DataTable();

            var columns = tableObject["Columns"];
            foreach (var column in columns)
            {
                string columnName = column["ColumnName"].ToString();
                string dataType = column["DataType"].ToString();
                dataTable.Columns.Add(columnName, Type.GetType(dataType));
            }

            var rows = tableObject["Rows"];
            foreach (var row in rows)
            {
                var dataRow = dataTable.NewRow();
                foreach (DataColumn column in dataTable.Columns)
                {
                    dataRow[column.ColumnName] = row[column.ColumnName].ToObject(column.DataType);
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
        
        private static DataTable BuildDataTable(string data)
        {
            var tableStructure = JsonConvert.DeserializeObject<TableStructure>(data);

            var dataTable = new DataTable();

            foreach (var column in tableStructure.Columns)
            {
                Type dataType = Type.GetType(column.DataType) ?? typeof(string);
                dataTable.Columns.Add(column.ColumnName, dataType);
            }

            foreach (var row in tableStructure.Rows)
            {
                var dataRow = dataTable.NewRow();

                foreach (var column in dataTable.Columns.Cast<DataColumn>())
                {
                    dataRow[column.ColumnName] = row[column.ColumnName];
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
        
        public class TableStructure
        {
            public List<Column> Columns { get; set; } = new();
            public List<Dictionary<string, object>> Rows { get; set; } = new();
        }

        public class Column
        {
            public string ColumnName { get; set; }
            public string DataType { get; set; }
        }

        #endregion
    }
};

