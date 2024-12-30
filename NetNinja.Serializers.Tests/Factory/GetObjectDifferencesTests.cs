using System.Reflection;

namespace NetNinja.Serializers.Tests.Factory
{
    public class GetObjectDifferencesTests
    {
        private static List<string> GetObjectDifferences<T>(T obj1, T obj2)
        {
            var differences = new List<string>();

            if (obj1 == null && obj2 == null)
            {
                return differences; 
            }

            if (obj1 == null)
            {
                differences.Add("Object 1 is null while Object 2 has a value.");
                return differences;
            }

            if (obj2 == null)
            {
                differences.Add("Object 2 is null while Object 1 has a value.");
                return differences;
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value1 = property.GetValue(obj1);
                var value2 = property.GetValue(obj2);

                if (!Equals(value1, value2))
                {
                    differences.Add($"Property '{property.Name}' differs: '{value1}' vs. '{value2}'");
                }
            }

            return differences;
        }

        private class TestObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Fact]
        public void GetObjectDifferences_BothObjectsAreNull_ReturnsEmptyList()
        {
            TestObject obj1 = null;
            TestObject obj2 = null;

            var differences = GetObjectDifferences(obj1, obj2);

            Assert.Empty(differences);
        }

        [Fact]
        public void GetObjectDifferences_OneObjectIsNull_ReturnsDifference()
        {
            TestObject obj1 = null;
            var obj2 = new TestObject { Id = 1, Name = "Test" };

            var differences = GetObjectDifferences(obj1, obj2);

            Assert.Single(differences);
            Assert.Contains("Object 1 is null while Object 2 has a value.", differences);
        }

        [Fact]
        public void GetObjectDifferences_ObjectsAreEqual_ReturnsEmptyList()
        {
            var obj1 = new TestObject { Id = 1, Name = "Test" };
            var obj2 = new TestObject { Id = 1, Name = "Test" };

            var differences = GetObjectDifferences(obj1, obj2);

            Assert.Empty(differences);
        }

        [Fact]
        public void GetObjectDifferences_ObjectsAreDifferent_ReturnsDifferences()
        {
            var obj1 = new TestObject { Id = 1, Name = "Test1" };
            var obj2 = new TestObject { Id = 2, Name = "Test2" };

            var differences = GetObjectDifferences(obj1, obj2);

            Assert.Equal(2, differences.Count);
            Assert.Contains("Property 'Id' differs: '1' vs. '2'", differences);
            Assert.Contains("Property 'Name' differs: 'Test1' vs. 'Test2'", differences);
        }

        [Fact]
        public void GetObjectDifferences_ObjectsHaveDefaultProperties_ReturnsEmptyList()
        {
            var obj1 = new TestObject(); 
            var obj2 = new TestObject(); 

            var differences = GetObjectDifferences(obj1, obj2);

            Assert.Empty(differences);
        }

        [Fact]
        public void GetObjectDifferences_ComplexPropertyReferenceDifference_ReturnsDifference()
        {
            var obj1 = new { Id = 1, SubObject = new TestObject { Id = 1, Name = "SubTest1" } };
            var obj2 = new { Id = 1, SubObject = new TestObject { Id = 1, Name = "SubTest1" } }; // Nueva instancia

            var differences = GetObjectDifferences(obj1, obj2);

            Assert.Single(differences);

            Assert.Contains("Property 'SubObject' differs:", differences[0]); 
        }

        [Fact]
        public void GetObjectDifferences_OneObjectHasNullProperty_ReturnsDifference()
        {
            var obj1 = new TestObject { Id = 1, Name = null };
            var obj2 = new TestObject { Id = 1, Name = "Test" };

            var differences = GetObjectDifferences(obj1, obj2);

            Assert.Single(differences);
            Assert.Contains("Property 'Name' differs: '' vs. 'Test'", differences);
        }
    }
};