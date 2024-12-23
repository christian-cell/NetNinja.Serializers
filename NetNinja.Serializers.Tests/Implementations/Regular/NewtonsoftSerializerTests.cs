using NetNinja.Serializers.Implementations.Regular;
using NetNinja.Serializers.Tests.Mocks;
using Xunit.Abstractions;

namespace NetNinja.Serializers.Tests.Implementations.Regular
{
    public class NewtonsoftSerializerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public NewtonsoftSerializerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        #region Serialize

        [Fact]
        public void Serialize_Should_return_argumentNullException_if_obj_is_not_provided()
        {
            NewtonsoftSerializer<PersonMockVersioned> bsonSerializerWithVersion = new NewtonsoftSerializer<PersonMockVersioned>(); 
            
            var exception = Assert.Throws<ArgumentNullException>(() => bsonSerializerWithVersion.Serialize(null));
            
            Assert.Equal("obj", exception.ParamName);
        }

        #endregion
    }
};

