using System.Data;
using NetNinja.Serializers.Implementations.ForHooks;
using Newtonsoft.Json;

namespace NetNinja.Serializers.Implementations.ForDataSets
{
    public class DataSetSerializerWithHooks : SerializerWithHooks<DataSet>
    {
        #region Sync Methods
        public override string Serialize(DataSet dataSet, string format = "Compact")
        {
            if (BeforeSerialize != null)
            {
                dataSet = BeforeSerialize.Invoke(dataSet);
            }

            Formatting formatting = format.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                ? Formatting.Indented
                : Formatting.None;

            return JsonConvert.SerializeObject(dataSet, formatting);
        }

        public override DataSet Deserialize(string data)
        {
            var dataSet = JsonConvert.DeserializeObject<DataSet>(data);

            if (AfterDeserialize != null)
            {
                dataSet = AfterDeserialize.Invoke(dataSet);
            }

            return dataSet;
        }

        public override string CombineSerialized(IEnumerable<DataSet> objects)
        {
            var serializedObjects = new List<string>();

            foreach (var obj in objects)
            {
                var transformedObj = BeforeSerialize?.Invoke(obj) ?? obj;
                serializedObjects.Add(JsonConvert.SerializeObject(transformedObj));
            }

            return "[" + string.Join(",", serializedObjects) + "]";
        }

        public override IEnumerable<DataSet> SplitSerialized(string combinedSerialized)
        {
            var serializedArray = JsonConvert.DeserializeObject<List<string>>(combinedSerialized) 
                                  ?? new List<string>();

            foreach (var item in serializedArray)
            {
                var obj = JsonConvert.DeserializeObject<DataSet>(item);
                yield return AfterDeserialize?.Invoke(obj) ?? obj; 
            }
        }
        #endregion

        #region Async Methods

        public override async Task<string> SerializeAsync(DataSet dataSet, string format = "Compact")
        {
            return await Task.Run(() =>
            {
                if (BeforeSerialize != null)
                {
                    dataSet = BeforeSerialize.Invoke(dataSet);
                }

                Formatting formatting = format.Equals("Indented", StringComparison.OrdinalIgnoreCase)
                    ? Formatting.Indented
                    : Formatting.None;

                return JsonConvert.SerializeObject(dataSet, formatting);
            });
        }

        #endregion
    }
};

