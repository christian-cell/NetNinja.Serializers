using NetNinja.Serializers.Abstractions;

namespace NetNinja.Serializers.Configurations
{
    public class VersionObject : IVersioned
    {

        public VersionObject()
        {
        }

        public VersionObject(string version, string name)
        {
            Version = version;
            Name = name;
        }

        public string Version { get; set; }
        public string Name { get; set; }
    }
};