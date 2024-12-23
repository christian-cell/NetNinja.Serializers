using NetNinja.Serializers.Abstractions;

namespace NetNinja.Serializers.Tests.Mocks
{
    public class PersonMockVersioned: IVersioned
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Skills { get; set; }
        public string Version { get; set; }
    }
};

