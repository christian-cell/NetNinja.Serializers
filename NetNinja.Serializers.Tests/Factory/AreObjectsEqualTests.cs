using System.Text.Json;

namespace NetNinja.Serializers.Tests.Factory
{
    public class AreObjectsEqualTests
    {
        private class TestObject
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Theory]
        [InlineData(null, null, true)] 
        [InlineData(null, "", false)] 
        public void Test_NullObjects(string obj1, string obj2, bool expected)
        {
            var result = AreObjectsEqual(obj1, obj2);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_EqualObjects()
        {
            var obj1 = new TestObject { Name = "John", Age = 30 };
            var obj2 = new TestObject { Name = "John", Age = 30 };

            var result = AreObjectsEqual(obj1, obj2);

            Assert.True(result);
        }

        [Fact]
        public void Test_DifferentObjects()
        {
            var obj1 = new TestObject { Name = "John", Age = 30 };
            var obj2 = new TestObject { Name = "Jane", Age = 25 };

            var result = AreObjectsEqual(obj1, obj2);

            Assert.False(result);
        }

        [Fact]
        public void Test_ObjectsWithDifferentJsonSerializationOrder()
        {
            var obj1 = new TestObject{ Name = "John", Age = 30 };
            var obj2 = new TestObject{ Age = 30, Name = "John" };

            var result = AreObjectsEqual(obj1, obj2);

            Assert.True(result); 
        }
    
        private static bool AreObjectsEqual<T>(T obj1, T obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                return obj1 == null && obj2 == null;
            }

            var json1 = JsonSerializer.Serialize(obj1);

            var json2 = JsonSerializer.Serialize(obj2);

            return string.Equals(json1, json2, StringComparison.Ordinal);
        }
    }
};

