namespace NetNinja.Serializers.Abstractions
{
    
    /*
     * Wen can guarranty the classes you wish serialize really implements this interface
     Sample : 
            public class ExampleDataV1 : IVersioned
           {
               public string Version { get; set; } = "1.0";
               public int Id { get; set; }
               public string Name { get; set; }
           }

           public class ExampleDataV2 : IVersioned
           {
               public string Version { get; set; } = "2.0";
               public string Name { get; set; }
               public string Description { get; set; }
           }
     */
    
    public interface IVersioned
    {
        string Version { get; set; }
    }
};

