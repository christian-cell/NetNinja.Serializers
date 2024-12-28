using Newtonsoft.Json;

namespace NetNinja.Serializers.Implementations.ForHooks
{
    public abstract class SerializerWithHooks<T>
    {
        public Func<T, T>? BeforeSerialize { get; set; }
        public Func<T, T>? AfterDeserialize { get; set; }
        
        // Serialización con formato opcional
        /*public override string Serialize(T obj , bool? encrypt = false , string format = "" )*/
        public abstract string Serialize(T obj , bool? encrypt = null , string format = ""  );

        // public abstract string Serialize(T obj);
        public abstract T Deserialize(string data);

        #region Sync Methods

        public virtual string CombineSerialized(IEnumerable<T> objects, bool? encrypt = null , string format = "")
        {
            var serializedObjects = objects.Select(obj =>
            {
                if (BeforeSerialize != null)
                {
                    var transformedObj = BeforeSerialize.Invoke(obj);
                    if (transformedObj is not null)
                    {
                        obj = transformedObj;
                    }
                }
                
                return Serialize(obj , false); 
            });

            return "[" + string.Join(",", serializedObjects) + "]"; 
        }

        public virtual IEnumerable<T> SplitSerialized(string combinedSerialized)
        {
            var serializedArray = JsonConvert.DeserializeObject<List<string>>(combinedSerialized); 

            if (serializedArray == null)
                throw new InvalidOperationException("Package combined is not valid or is empty pls check again");

            return serializedArray.Select(serializedData =>
            {
                var obj = Deserialize(serializedData);
                
                if (AfterDeserialize != null)
                {
                    var transformedObj = AfterDeserialize.Invoke(obj);
                    if (transformedObj is not null)
                    {
                        return transformedObj;
                    }
                }
                
                return obj;
            });
        }

        #endregion

        #region Async Methods

        public virtual async Task<T> DeserializeAsync(string data)
        {
            return await Task.Run(() =>
            {
                return Deserialize(data);
            });
        }

        public virtual async Task<string> CombineSerializedAsync(IEnumerable<T> objects)
        {
            return await Task.Run(() =>
            {
                return CombineSerialized(objects);
            });
        }

        public virtual async Task<IEnumerable<T>> SplitSerializedAsync(string combinedSerialized)
        {
            return await Task.Run(() =>
            {
                return SplitSerialized(combinedSerialized);
            });
        }
        
        public virtual async Task<string> SerializeAsync(T obj, bool? encrypt = null, string format = "")
        {
            return await Task.Run(() => Serialize(obj, encrypt , format));
        }

        #endregion
    }
};

