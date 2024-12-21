# NetNinja.Serializers

**NetNinja.Serializers** is a library for handling **serialization** and **deserialization** of objects with support for multiple formats, using versioned objects. This tool is designed to be extensible and flexible, simplifying data management across different formats.

## Key Features

- **Support for multiple serialization formats:**
  - JSON (using `System.Text.Json`)
  - JSON (using `Newtonsoft.Json`)
  - XML
  - YAML
  - BSON

- **Versioned object support:** The serializers support the serialization and deserialization of versioned objects through a defined contract via the `IVersioned` interface.

- **Serializer Factory:** Provides a central point to retrieve specific serializers by format, register custom serializers, and perform advanced actions like converting data between formats and comparing serialized information.

---

## Index

## Factory
- [Register Versioned Serializer](#register-versioned-serializer)
- [Supported Data Types](#supported-data-types)
- [What is IVersioned?](#what-is-iversioned)
- [Get Versioned Serializer](#get-versioned-serializer)
- [Convert Between Formats](#convert-between-formats)
- [Compare Serialized Data](#compare-serialized-data)
- [Get Differences](#get-differences)

## For Datasets
- [Serialize with hooks](#serialize-with-hooks)
- [Deserialize with hooks](#deserialize-with-hooks)
- [Combine serialized](#combine-serialized)
- [Split serialized](#split-serialized)
- [Async serialization](#async-serialization)

## Data Tables With Hooks
- [Serialize data tables sync](#serialize-data-tables-sync)
- [Deserialize data tables sync](#deserialize-data-tables-sync)
- [Combine serialize for data tables](#combine-serialize-for-data-tables)
- [Split serialized for data tables](#split-serialized-for-data-tables)
- [Serialize async for data tables](#serialize-async-for-data-tables)
- [Deserialize async for data tables](#deserialize-async-for-data-tables)
- [Combine serialized Async for data tables](#combine-serialized-Async-for-data-tables)
- [Split serialized Async for data tables](#Split-serialized-Async-for-data-tables)
- [Save json in file async](#save-json-in-file-async)
- [Deserialize from file](#deserialize-from-file)

## For Dynamics
- [Serialize dynamic data](#serialize-dynamic-data)
- [Deserialize dynamic data](#deserialize-dynamic-data)


## BSON For Hooks
- [Bson serialize](#bson-serialize)
- [Bson deserialize](#bson-deserialize)
- [Bson combine serialized](#bson-combine-serialize)
- [Bson split serialized](#bson-split-serialized)
- [Bson async serialize](#bson-async-serialize)

## JsonSerializerWithHooks For Hooks
- [Json serializer with hooks](#json-serializer-with-hooks)
- [Json deserializer with hooks](#json-deserializer-with-hooks)
- [Json async serializer with hooks](#json-async-serializer-with-hooks)
- [Json async deserializer with hooks](#json-async-deserializer-with-hooks)

## SerializerXml For Hooks
- [Serialize xml with hooks](#serialize-xml-with-hooks)
- [Derialize xml with hooks](#deserialize-xml-with-hooks)

## Yaml's For Hooks
- [Serialize yaml with hooks](#Serialize-yaml-with-hooks)
- [Deserialize yaml with hooks](#Deserialize-yaml-with-hooks)

## Networks
- [Serialize message from network](#Serialize-message-from-network)
- [Deserialize message from network](#Deserialize-message-from-network)

## Regulars
- [Beson Serialize and deserialize](#beson-serialize-and-deserialize)
- [NewtonSoft Serialize and deserialize](#newtonSoft-serialize-and-deserialize)
- [SystemText Serialize and deserialize](#systemtext-serialize-and-deserialize)
- [Xml Serialize and deserialize](#xml-serialize-and-deserialize)
- [Yaml Serialize and deserialize](#yaml-serialize-and-deserialize)

## Versioned
- [Beson versioned serialize and deserialize](#Beson-versioned-serialize-and-deserialize)
- [Newtonsoft versioned serialize and deserialize](#newtonsoft-versioned-serialize-and-deserialize)
- [SystemText versioned serialize and deserialize](#systemtext-versioned-serialize-and-deserialize)
- [Xml versioned serialize and deserialize](#xml-versioned-serialize-and-deserialize)
- [Yaml versioned serialize and deserialize](#yaml-versioned-serialize-and-deserialize)

## With Destinations
- [Serialize and Deserialize with destination](#Serialize-and-deserialize-with-destination)

## Encryption
- [Send the data to serialize and deserialize encripted](#Send-the-data-to-serialize-and-deserialize-encripted)


## Installation
- [Installation](#installation)

## License
- [License](#license)

## Contributing
- [Contributing](#contributing)

---

## Register Versioned Serializer

This section explains how to register a versioned serializer with the library using the `RegisterVersioned` method. You can add custom serializers for any specific data format by associating it with a factory that creates an instance of your serializer.

Here’s an example:

```csharp
using NetNinja.Serializers.Factories; 

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RegisterCustomSerializer();

            Console.WriteLine("Registration completed.");
        }

        static void RegisterCustomSerializer()
        {
            SerializerFactory.RegisterVersioned("CustomJson", type =>
            {
                if (type == typeof(MyCustomSerializer))
                {
                }
                
                throw new NotSupportedException($"Serializer not supported for the type: {type}");
            });

            Console.WriteLine("A new serializer for 'CustomJson' has been successfully registered.");
        }
    }

    public class MyCustomSerializer
    {
        public string Serialize(object obj)
        {
            return $"[CustomJson: {obj.ToString()}]";
        }

        public object Deserialize(string data)
        {
            return $"[Deserialized: {data}]";
        }
    }
}
```

When you click the **Register Versioned Serializer** item in the index, you will be redirected to this section.

---

## Supported Data Types

The methods in this library are designed to work with objects that implement the `IVersioned` interface. This ensures that each versioned object can be handled and remain compatible through various transformations.

### What is `IVersioned`?

`IVersioned` is an interface that defines the contract for all versioned objects. It includes the `Version` property, which is used by the library to ensure compatibility during serialization or deserialization.

Here’s how `IVersioned` is defined in the project:

```csharp
public interface IVersioned
{
    string Version { get; set; }
}
```

Any object implementing this interface can be used in serialization, deserialization, comparison, and conversion operations.

### Get Versioned Serializer

```csharp
using NetNinja.Serializers.Abstractions; // Use the correct namespace for the library's IVersioned interface
using NetNinja.Serializers.Factories;    // Use the namespace where SerializerFactory is defined

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Register a custom serializer
                RegisterCustomSerializer();

                // Retrieve the registered serializer
                var serializer = SerializerFactory.GetVersionedSerializer<MyCustomObject>("CustomJson");

                // Example object to serialize
                var myObject = new MyCustomObject
                {
                    Version = "1.0",
                    Name = "Test Object",
                    Id = 123
                };

                // Serialize the object
                var serializedData = serializer.Serialize(myObject);
                Console.WriteLine($"Serialized data: {serializedData}");

                // Deserialize the data back into an object
                var deserializedObject = serializer.Deserialize(serializedData);
                Console.WriteLine($"Deserialized data: [Version: {deserializedObject.Version}, Name: {deserializedObject.Name}, Id: {deserializedObject.Id}]");
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void RegisterCustomSerializer()
        {
            // Use the RegisterVersioned method to add a new custom serializer
            SerializerFactory.RegisterVersioned("CustomJson", type =>
            {
                if (type == typeof(MyCustomObject))
                {
                    return new MyCustomSerializer();
                }

                throw new NotSupportedException($"Serializer not supported for the type: {type}");
            });

            Console.WriteLine("A new serializer for 'CustomJson' has been successfully registered.");
        }
    }

    // Define a custom object that implements the IVersioned interface from the library
    public class MyCustomObject : IVersioned
    {
        public string Version { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }

    // Create a custom serializer for MyCustomObject
    public class MyCustomSerializer : ISerializer<MyCustomObject>
    {
        // Serialize the object (example implementation)
        public string Serialize(MyCustomObject obj)
        {
            return $"{{\"Version\":\"{obj.Version}\",\"Name\":\"{obj.Name}\",\"Id\":{obj.Id}}}";
        }

        // Deserialize the data to a MyCustomObject (example implementation)
        public MyCustomObject Deserialize(string data)
        {
            // Note: This is a simplified implementation for demonstration purposes
            // In a real scenario, you would parse the JSON properly.
            var version = data.Substring(data.IndexOf("\"Version\":\"") + 10, 3);
            var name = data.Substring(data.IndexOf("\"Name\":\"") + 8, data.IndexOf("\",\"Id\":") - (data.IndexOf("\"Name\":\"") + 8));
            var id = int.Parse(data.Substring(data.IndexOf("\"Id\":") + 5, 3));
            
            return new MyCustomObject { Version = version, Name = name, Id = id };
        }
    }
}
```

### Convert Between Formats
```csharp
using System;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Factories;

namespace NetNinja.Serializers.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Sample JSON source data
                string sourceData = "{\"Name\":\"John\",\"Age\":30,\"Version\":\"1.0\"}";

                // Convert the data from JSON to XML using the SerializerFactory
                string targetData = SerializerFactory.ConvertBetweenFormats<ExampleVersioned>(
                    sourceData,
                    "JSON",
                    "XML"
                );

                // Print the converted data
                Console.WriteLine("Converted Data (XML Format):");
                Console.WriteLine(targetData);
            }
            catch (Exception ex)
            {
                // Handling any exceptions that may occur during the conversion
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }

        // Example of a versioned object used for serialization
        public class ExampleVersioned : IVersioned
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Version { get; set; } // Required by IVersioned
        }
    }
}
```

### Compare Serialized Data
```csharp
using System;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Factories;

namespace NetNinja.Serializers.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Source data in JSON format
                string jsonSourceData = "{\"Name\":\"John\",\"Age\":30,\"Version\":\"1.0\"}";

                // Data in XML format representing the same object
                string xmlSourceData = 
                    "<ExampleVersioned>" +
                    "<Name>John</Name>" +
                    "<Age>30</Age>" +
                    "<Version>1.0</Version>" +
                    "</ExampleVersioned>";

                // Compare the serialized data
                bool areEqual = SerializerFactory.CompareSerializedData<ExampleVersioned>(
                    jsonSourceData,
                    "JSON",
                    xmlSourceData,
                    "XML"
                );

                // Print the result of comparison
                Console.WriteLine(areEqual
                    ? "The serialized data represent the same object."
                    : "The serialized data do not represent the same object.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions during the comparison
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }

        // Example of a versioned object used in serialization
        public class ExampleVersioned : IVersioned
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Version { get; set; } // Required by IVersioned
        }
    }
}
```

### Get Differences
```csharp
using System;
using System.Collections.Generic;
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Factories;

namespace NetNinja.Serializers.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Source data in JSON format
                string jsonSourceData = "{\"Name\":\"John\",\"Age\":30,\"Version\":\"1.0\"}";

                // Data in XML format with slight differences
                string xmlSourceData = 
                    "<ExampleVersioned>" +
                    "<Name>Johnny</Name>" + // Difference here (Name field)
                    "<Age>31</Age>" +       // Difference here (Age field)
                    "<Version>1.0</Version>" +
                    "</ExampleVersioned>";

                // Get differences between the two serialized data sets
                List<string> differences = SerializerFactory.GetDifferences<ExampleVersioned>(
                    jsonSourceData,
                    "JSON",
                    xmlSourceData,
                    "XML"
                );

                // Print differences
                if (differences.Count == 0)
                {
                    Console.WriteLine("No differences found between the objects.");
                }
                else
                {
                    Console.WriteLine("Differences found:");
                    foreach (var difference in differences)
                    {
                        Console.WriteLine(difference);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions during the comparison
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }

        // Example of a versioned object used in serialization
        public class ExampleVersioned : IVersioned
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Version { get; set; } // Required by IVersioned
        }
    }
}
```
output :
```
Differences found:
Property 'Name' differs: 'John' vs. 'Johnny'
Property 'Age' differs: '30' vs. '31'
```
---

### Serialize with hooks
You can add hooks (`BeforeSerialize` and `AfterDeserialize`) if you need them
```csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataSet = new DataSet("ExampleDataSet");
            var dataTable = new DataTable("ExampleTable");

            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));

            dataTable.Rows.Add(1, "Alice");
            dataTable.Rows.Add(2, "Bob");
            dataTable.Rows.Add(3, "Charlie");

            dataSet.Tables.Add(dataTable);

            var serializer = new DataSetSerializerWithHooks();

            serializer.BeforeSerialize = ds =>
            {
                Console.WriteLine("BeforeSerialize hook invoked.");
                ds.DataSetName = "ModifiedDataSet";
                return ds;
            };

            Console.WriteLine("Serializing DataSet...");
            string serializedData = serializer.Serialize(dataSet, "Indented");

            Console.WriteLine("Serialized Data:");
            Console.WriteLine(serializedData);
        }
    }
}
```
output:
```
Serializing DataSet...
BeforeSerialize hook invoked.
Serialized Data:
{
  "ExampleTable": [
    {
      "Id": 1,
      "Name": "Alice"
    },
    {
      "Id": 2,
      "Name": "Bob"
    },
    {
      "Id": 3,
      "Name": "Charlie"
    }
  ]
}

```

### Deserialize with hooks
```csharp
using System;
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string datasetJson = @"
            {
              ""Tables"": [
                {
                  ""TableName"": ""ExampleTable"",
                  ""Columns"": [
                    { ""ColumnName"": ""Id"", ""DataType"": ""System.Int32"" },
                    { ""ColumnName"": ""Name"", ""DataType"": ""System.String"" }
                  ],
                  ""Rows"": [
                    { ""Id"": 1, ""Name"": ""Alice"" },
                    { ""Id"": 2, ""Name"": ""Bob"" },
                    { ""Id"": 3, ""Name"": ""Charlie"" }
                  ]
                }
              ]
            }";

            var serializer = new DataSetSerializerWithHooks();

            serializer.AfterDeserialize = ds =>
            {
                Console.WriteLine("AfterDeserialize hook invoked.");

                if (ds.Tables.Contains("ExampleTable"))
                {
                    var table = ds.Tables["ExampleTable"];
                    table.Rows.Add(4, "Diana");
                }
                return ds;
            };

            Console.WriteLine("Deserializing JSON to DataSet...");
            DataSet deserializedDataSet = serializer.Deserialize(datasetJson);

            Console.WriteLine("Deserialized DataSet:");
            PrintDataSet(deserializedDataSet);
        }

        static void PrintDataSet(DataSet dataSet)
        {
            Console.WriteLine($"DataSet Name: {dataSet.DataSetName}");
            foreach (DataTable table in dataSet.Tables)
            {
                Console.WriteLine($"Table: {table.TableName}");
                foreach (DataColumn column in table.Columns)
                {
                    Console.Write($"{column.ColumnName}\t");
                }
                Console.WriteLine();
                foreach (DataRow row in table.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.Write($"{item}\t");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
```
Output
```
Deserializing JSON to DataSet...
AfterDeserialize hook invoked.
Deserialized DataSet:
DataSet Name: NewDataSet
Table: Tables
TableName       Columns Rows
ExampleTable
```

### Combine serialized
```csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataSet1 = new DataSet("DataSet1");
            var dataTable1 = new DataTable("Table1");
            dataTable1.Columns.Add("Id", typeof(int));
            dataTable1.Columns.Add("Name", typeof(string));
            dataTable1.Rows.Add(1, "Alice");
            dataTable1.Rows.Add(2, "Bob");
            dataSet1.Tables.Add(dataTable1);

            var dataSet2 = new DataSet("DataSet2");
            var dataTable2 = new DataTable("Table2");
            dataTable2.Columns.Add("Id", typeof(int));
            dataTable2.Columns.Add("Name", typeof(string));
            dataTable2.Rows.Add(3, "Charlie");
            dataTable2.Rows.Add(4, "Diana");
            dataSet2.Tables.Add(dataTable2);

            var dataSets = new List<DataSet> { dataSet1, dataSet2 };

            var serializer = new DataSetSerializerWithHooks();

            serializer.BeforeSerialize = ds =>
            {
                Console.WriteLine($"BeforeSerialize hook invoked for {ds.DataSetName}.");
                ds.ExtendedProperties["LastModified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                return ds;
            };

            Console.WriteLine("Combining serialized DataSets...");
            string combinedJson = serializer.CombineSerialized(dataSets);

            Console.WriteLine("Combined JSON:");
            Console.WriteLine(combinedJson);
        }
    }
}
```
Output
```
Combining serialized DataSets...
BeforeSerialize hook invoked for DataSet1.
BeforeSerialize hook invoked for DataSet2.
Combined JSON:
[{"Table1":[{"Id":1,"Name":"Alice"},{"Id":2,"Name":"Bob"}]},{"Table2":[{"Id":3,"Name":"Charlie"},{"Id":4,"Name":"Diana"}]}]
```

### Split serialized
This method is perfect for situations where data is grouped and need be provided of modular effcient and custom way
```csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;
using Newtonsoft.Json;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataSet1 = new DataSet("DataSet1");
            var table1 = new DataTable("Table1");
            table1.Columns.Add("Id", typeof(int));
            table1.Columns.Add("Name", typeof(string));
            table1.Rows.Add(1, "Alice");
            table1.Rows.Add(2, "Bob");
            dataSet1.Tables.Add(table1);

            var dataSet2 = new DataSet("DataSet2");
            var table2 = new DataTable("Table2");
            table2.Columns.Add("Id", typeof(int));
            table2.Columns.Add("Name", typeof(string));
            table2.Rows.Add(3, "Charlie");
            table2.Rows.Add(4, "Diana");
            dataSet2.Tables.Add(table2);

            var serializedDataSets = new List<string>
            {
                JsonConvert.SerializeObject(dataSet1),
                JsonConvert.SerializeObject(dataSet2)
            };

            string combinedSerializedJson = JsonConvert.SerializeObject(serializedDataSets);

            Console.WriteLine("Combined Serialized JSON for testing:");
            Console.WriteLine(combinedSerializedJson);

            var serializer = new DataSetSerializerWithHooks();

            // Hook AfterDeserialize to modify DataSet after deserialized
            serializer.AfterDeserialize = ds =>
            {
                Console.WriteLine($"AfterDeserialize hook invoked for {ds.DataSetName}");
                return ds; 
            };

            Console.WriteLine("\nSplitting combined serialized JSON...");
            var dataSets = serializer.SplitSerialized(combinedSerializedJson);

            foreach (var dataSet in dataSets)
            {
                Console.WriteLine($"Deserialized DataSet: {dataSet.DataSetName}");
                foreach (DataTable table in dataSet.Tables)
                {
                    Console.WriteLine($"  Table: {table.TableName}");
                    foreach (DataRow row in table.Rows)
                    {
                        foreach (var item in row.ItemArray)
                        {
                            Console.Write($"{item}\t");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}
```
Output
```
Combined Serialized JSON for testing:
["{\"Table1\":[{\"Id\":1,\"Name\":\"Alice\"},{\"Id\":2,\"Name\":\"Bob\"}]}","{\"Table2\":[{\"Id\":3,\"Name\":\"Charlie\"},{\"Id\":4,\"Name\":\"Diana\"}]}"]


Splitting combined serialized JSON...
AfterDeserialize hook invoked for NewDataSet
Deserialized DataSet: NewDataSet
  Table: Table1
1       Alice
2       Bob
AfterDeserialize hook invoked for NewDataSet
Deserialized DataSet: NewDataSet
  Table: Table2
3       Charlie
4       Diana

```

### Async serialization
```csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

class Program
{
    static async Task Main(string[] args)
    {
        var dataSet = new DataSet("ExampleDataSet");
        var table = new DataTable("ExampleTable");
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add(1, "Alice");
        table.Rows.Add(2, "Bob");
        dataSet.Tables.Add(table);

        var serializer = new DataSetSerializerWithHooks();
        string json = await serializer.SerializeAsync(dataSet, "Indented");

        Console.WriteLine("Serialized JSON:");
        Console.WriteLine(json);
    }
}
```
Output
```
Serialized JSON:
{
  "ExampleTable": [
    {
      "Id": 1,
      "Name": "Alice"
    },
    {
      "Id": 2,
      "Name": "Bob"
    }
  ]
}

```

### Serialize data tables sync
```csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

class Program
{
    static void Main(string[] args)
    {
        var dataTable = new DataTable("ExampleTable");
        
        dataTable.Columns.Add("Id", typeof(int));
        dataTable.Columns.Add("Name", typeof(string));
        
        dataTable.Rows.Add(1, "Alice");
        dataTable.Rows.Add(2, "Bob");
        dataTable.Rows.Add(3, "Charlie");

        var serializer = new DataTableSerializerWithHooks();

        string jsonCompact = serializer.Serialize(dataTable);
        Console.WriteLine("Serialized JSON (Compact):");
        Console.WriteLine(jsonCompact);

        string jsonIndented = serializer.Serialize(dataTable, "Indented");
        Console.WriteLine("\nSerialized JSON (Indented):");
        Console.WriteLine(jsonIndented);
    }
}
```
Output
```
Serialized JSON (Compact):
[{"Id":1,"Name":"Alice"},{"Id":2,"Name":"Bob"},{"Id":3,"Name":"Charlie"}]

Serialized JSON (Indented):
[
  {
    "Id": 1,
    "Name": "Alice"
  },
  {
    "Id": 2,
    "Name": "Bob"
  },
  {
    "Id": 3,
    "Name": "Charlie"
  }

```

### Deserialize data tables sync
```csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

class Program
{
    static void Main(string[] args)
    {
        string json = @"
        {
          ""TableName"": ""ExampleTable"",
          ""Columns"": [
            { ""ColumnName"": ""Id"", ""DataType"": ""System.Int32"" },
            { ""ColumnName"": ""Name"", ""DataType"": ""System.String"" }
          ],
          ""Rows"": [
            { ""Id"": 1, ""Name"": ""Alice"" },
            { ""Id"": 2, ""Name"": ""Bob"" },
            { ""Id"": 3, ""Name"": ""Charlie"" }
          ]
        }";

        var serializer = new DataTableSerializerWithHooks();

        // add the hook AfterDeserialize to transform the DataTable
        serializer.AfterDeserialize = dt =>
        {
            // Filter rows where Id greater 2
            DataTable filteredTable = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                if ((int)row["Id"] > 2)
                {
                    filteredTable.ImportRow(row);
                }
            }
            return filteredTable;
        };

        // deserialize JSON in a DataTable
        DataTable dataTable = serializer.Deserialize(json);

        // show filtered data
        Console.WriteLine("Deserialized DataTable (Filtered by AfterDeserialize):");
        Console.WriteLine($"TableName: {dataTable.TableName}");
        Console.WriteLine("Columns:");
        foreach (DataColumn column in dataTable.Columns)
        {
            Console.WriteLine($"- {column.ColumnName} ({column.DataType.Name})");
        }
        Console.WriteLine("Rows:");
        foreach (DataRow row in dataTable.Rows)
        {
            Console.WriteLine($"- Id: {row["Id"]}, Name: {row["Name"]}");
        }
    }
}
```
Output
```
Serialized JSON (Compact):
[{"Id":1,"Name":"Alice"},{"Id":2,"Name":"Bob"},{"Id":3,"Name":"Charlie"}]

Serialized JSON (Indented):
[
  {
    "Id": 1,
    "Name": "Alice"
  },
  {
    "Id": 2,
    "Name": "Bob"
  },
  {
    "Id": 3,
    "Name": "Charlie"
  }
]

```

### Combine serialize for data tables
````csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

class Program
{
    static void Main(string[] args)
    {
        // Create empty DataTable
        var emptyDataTable = new DataTable("EmptyTable");

        // Create populated DataTable
        var populatedDataTable = new DataTable("PopulatedTable");
        populatedDataTable.Columns.Add("Id", typeof(int));
        populatedDataTable.Columns.Add("Value", typeof(string));
        populatedDataTable.Rows.Add(1, "One");
        populatedDataTable.Rows.Add(2, "Two");

        // Add both data tables to a list
        var dataTables = new List<DataTable> { emptyDataTable, populatedDataTable };

        // get serializer
        var serializer = new DataTableSerializerWithHooks();

        // Combire the data tables into a single JSON string
        string combinedJson = serializer.CombineSerialized(dataTables);

        // Print results
        Console.WriteLine("Combined Serialized JSON with an Empty Table:");
        Console.WriteLine(combinedJson);
    }
}
````
Output
```
Combined Serialized JSON with an Empty Table:
[[],[{"Id":1,"Value":"One"},{"Id":2,"Value":"Two"}]]
```

### Split serialized for data tables
````csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

public class Program
{
    public static void Main()
    {
        string combinedSerialized = @"
        [
            ""{\""TableName\"":\""FirstTable\"",\""Columns\"":[{\""ColumnName\"":\""Id\"",\""DataType\"":\""System.Int32\""},
            {\""ColumnName\"":\""Name\"",\""DataType\"":\""System.String\""}],\""Rows\"":[{\""Id\"":1,\""Name\"":\""Alice\""},
            {\""Id\"":2,\""Name\"":\""Bob\""}]}"",
            ""{\""TableName\"":\""SecondTable\"",\""Columns\"":[{\""ColumnName\"":\""Code\"",\""DataType\"":\""System.String\""},
            {\""ColumnName\"":\""Value\"",\""DataType\"":\""System.Decimal\""}],\""Rows\"":[{\""Code\"":\""A1\"",\""Value\"":100.5},
            {\""Code\"":\""B1\"",\""Value\"":200.75}]}""
        ]";

        var dataTables = DataTableSerializerWithHooks.SplitSerialized(combinedSerialized);

        foreach (var table in dataTables)
        {
            Console.WriteLine($"Table: {table.TableName}");
            
            foreach (DataColumn column in table.Columns)
            {
                Console.Write($"{column.ColumnName}\t");
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item}\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine(new string('-', 40));
        }
    }
  
}
````
Output
```
Table:
Id      Name
1       Alice
2       Bob
----------------------------------------
Table:
Code    Value
A1      100,5
B1      200,75
----------------------------------------
```

### Serialize async for data tables
````csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create a DataTable with sample data
        var dataTable = new DataTable("ExampleTable");
        dataTable.Columns.Add("Id", typeof(int));
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Age", typeof(int));

        dataTable.Rows.Add(1, "Alice", 25);
        dataTable.Rows.Add(2, "Bob", 30);
        dataTable.Rows.Add(3, "Charlie", 35);

        // call serializer
        var serializer = new DataTableSerializerWithHooks();

        // Define the optional event BeforeSerialize
        serializer.BeforeSerialize = table =>
        {
            // Verify if the third row exists before delete it
            if (table.Rows.Count > 2) 
            {
                table.Rows.RemoveAt(2);
            }
            return table;
        };

        // Serialize data in compact format
        string compactJson = await serializer.SerializeAsync(dataTable, "Compact");
        Console.WriteLine("Compact JSON:");
        Console.WriteLine(compactJson);

        // Serialize the dataTable in indented format
        string indentedJson = await serializer.SerializeAsync(dataTable, "Indented");
        Console.WriteLine("\nIndented JSON:");
        Console.WriteLine(indentedJson);
    }
}
````
Output
```
Compact JSON:
[{"Id":1,"Name":"Alice","Age":25},{"Id":2,"Name":"Bob","Age":30}]

Indented JSON:
[
  {
    "Id": 1,
    "Name": "Alice",
    "Age": 25
  },
  {
    "Id": 2,
    "Name": "Bob",
    "Age": 30
  }
]

```

### Deserialize async for data tables
````csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

public class Program
{
    public static async Task Main(string[] args)
    {
        // sample JSON 
        string json = @"
        [
            { ""Id"": 1, ""Name"": ""Alice"", ""Age"": 25 },
            { ""Id"": 2, ""Name"": ""Bob"", ""Age"": 30 },
            { ""Id"": 3, ""Name"": ""Charlie"", ""Age"": 35 }
        ]";

        // Create an instance of deserializer
        var serializer = new DataTableSerializerWithHooks();

        // Define the event of optional AfterDeserialize
        serializer.AfterDeserialize = table =>
        {
            // Modify the DataTable after deserialize
            if (table.Columns.Contains("Age"))
            {
                foreach (DataRow row in table.Rows)
                {
                    // Add a new value calculated based in Age
                    row["Age"] = Convert.ToInt32(row["Age"]) + 5; // Add 5 years to each age
                }
            }
            return table;
        };

        // Call DeserializeAsync to convert the JSON in a DataTable
        DataTable dataTable = await serializer.DeserializeAsync(json);

        // Show content of DataTable deserialized
        Console.WriteLine("DataTable deserializado:");
        
        foreach (DataColumn column in dataTable.Columns)
        {
            Console.Write($"{column.ColumnName}\t");
        }

        Console.WriteLine();

        foreach (DataRow row in dataTable.Rows)
        {
            foreach (var item in row.ItemArray)
            {
                Console.Write($"{item}\t");
            }
            Console.WriteLine();
        }
    }
}
````
Output
```
DataTable deserializado:

Id      Name    Age
1       Alice   30
2       Bob     35
3       Charlie 40

```

### Combine serialized Async for data tables
````csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create two sample DataTables 
        var dataTable1 = new DataTable("Table1");
        dataTable1.Columns.Add("Id", typeof(int));
        dataTable1.Columns.Add("Name", typeof(string));

        dataTable1.Rows.Add(1, "Alice");
        dataTable1.Rows.Add(2, "Bob");

        var dataTable2 = new DataTable("Table2");
        dataTable2.Columns.Add("Code", typeof(string));
        dataTable2.Columns.Add("Value", typeof(decimal));

        dataTable2.Rows.Add("A1", 100.5m);
        dataTable2.Rows.Add("B1", 200.75m);

        // Create an instance of the custom serializer
        var serializer = new DataTableSerializerWithHooks();

        // Call the methos whit both dataTables
        string combinedJson = await serializer.CombineSerializedAsync(new List<DataTable> { dataTable1, dataTable2 });

        // Show results
        Console.WriteLine("Combined JSON:");
        Console.WriteLine(combinedJson);
    }
}
````
Output
```
Combined JSON:
[[{"Id":1,"Name":"Alice"},{"Id":2,"Name":"Bob"}],[{"Code":"A1","Value":100.5},{"Code":"B1","Value":200.75}]]
```

### Split serialized Async for data tables
````csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

public class Program
{
    public static async Task Main(string[] args)
    {
        string combinedSerialized = @"
        [
            ""{\""Columns\"": [{\""ColumnName\"": \""ProductId\"", \""DataType\"": \""System.Int32\""}, {\""ColumnName\"": \""ProductName\"", \""DataType\"": \""System.String\""}],
                \""Rows\"": [
                    {\""ProductId\"": 1, \""ProductName\"": \""Laptop\""},
                    {\""ProductId\"": 2, \""ProductName\"": \""Tablet\""},
                    {\""ProductId\"": 3, \""ProductName\"": \""Smartphone\""},
                    {\""ProductId\"": 4, \""ProductName\"": \""Monitor\""},
                    {\""ProductId\"": 5, \""ProductName\"": \""Keyboard\""}
                ]}"",

            ""{\""Columns\"": [{\""ColumnName\"": \""SalesId\"", \""DataType\"": \""System.Int32\""}, {\""ColumnName\"": \""Amount\"", \""DataType\"": \""System.Decimal\""}],
                \""Rows\"": [
                    {\""SalesId\"": 101, \""Amount\"": 500.75},
                    {\""SalesId\"": 102, \""Amount\"": 300.25},
                    {\""SalesId\"": 103, \""Amount\"": 600.50},
                    {\""SalesId\"": 104, \""Amount\"": 800.00},
                    {\""SalesId\"": 105, \""Amount\"": 450.30}
                ]}"",

            ""{\""Columns\"": [{\""ColumnName\"": \""CustomerId\"", \""DataType\"": \""System.Int32\""}, {\""ColumnName\"": \""CustomerName\"", \""DataType\"": \""System.String\""}],
                \""Rows\"": [
                    {\""CustomerId\"": 201, \""CustomerName\"": \""John Doe\""},
                    {\""CustomerId\"": 202, \""CustomerName\"": \""Jane Smith\""},
                    {\""CustomerId\"": 203, \""CustomerName\"": \""Alice Johnson\""},
                    {\""CustomerId\"": 204, \""CustomerName\"": \""Robert Brown\""},
                    {\""CustomerId\"": 205, \""CustomerName\"": \""Emily Davis\""}
                ]}"",

            ""{\""Columns\"": [{\""ColumnName\"": \""OrderId\"", \""DataType\"": \""System.Int32\""}, {\""ColumnName\"": \""OrderDate\"", \""DataType\"": \""System.DateTime\""}],
                \""Rows\"": [
                    {\""OrderId\"": 301, \""OrderDate\"": \""2023-01-15T00:00:00\""},
                    {\""OrderId\"": 302, \""OrderDate\"": \""2023-02-10T00:00:00\""},
                    {\""OrderId\"": 303, \""OrderDate\"": \""2023-03-05T00:00:00\""},
                    {\""OrderId\"": 304, \""OrderDate\"": \""2023-04-01T00:00:00\""},
                    {\""OrderId\"": 305, \""OrderDate\"": \""2023-05-20T00:00:00\""}
                ]}"",

            ""{\""Columns\"": [{\""ColumnName\"": \""CategoryId\"", \""DataType\"": \""System.Int32\""}, {\""ColumnName\"": \""CategoryName\"", \""DataType\"": \""System.String\""}],
                \""Rows\"": [
                    {\""CategoryId\"": 401, \""CategoryName\"": \""Electronics\""},
                    {\""CategoryId\"": 402, \""CategoryName\"": \""Clothing\""},
                    {\""CategoryId\"": 403, \""CategoryName\"": \""Groceries\""},
                    {\""CategoryId\"": 404, \""CategoryName\"": \""Furniture\""},
                    {\""CategoryId\"": 405, \""CategoryName\"": \""Books\""}
                ]}""
        ]
        ";

        // call splitSerialized to splic the combined JSON
        var tables = await DataTableSerializerWithHooks.SplitSerializedAsync(combinedSerialized);

        // Show results
        int tableIndex = 1;
        foreach (var table in tables)
        {
            Console.WriteLine($"DataTable {tableIndex++}:");
            foreach (DataColumn column in table.Columns)
            {
                Console.Write($"{column.ColumnName}\t");
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item}\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine(new string('-', 40));
        }
    }
}
````
Output
```
DataTable 1:
ProductId       ProductName
1       Laptop
2       Tablet
3       Smartphone
4       Monitor
5       Keyboard
----------------------------------------
DataTable 2:
SalesId Amount
101     500,75
102     300,25
103     600,5
104     800
105     450,3
----------------------------------------
DataTable 3:
CustomerId      CustomerName
201     John Doe
202     Jane Smith
203     Alice Johnson
204     Robert Brown
205     Emily Davis
----------------------------------------
DataTable 4:
OrderId OrderDate
301     15/01/2023 0:00:00
302     10/02/2023 0:00:00
303     05/03/2023 0:00:00
304     01/04/2023 0:00:00
305     20/05/2023 0:00:00
----------------------------------------
DataTable 5:
CategoryId      CategoryName
401     Electronics
402     Clothing
403     Groceries
404     Furniture
405     Books
----------------------------------------

```

### Save json in file async
````csharp
using NetNinja.Serializers.Implementations.ForDataSets;

public class Program
{
    public static async Task Main(string[] args)
    {
        string combinedSerialized = @"
        [
            ""{\""Columns\"": [{\""ColumnName\"": \""ProductId\"", \""DataType\"": \""System.Int32\""}, {\""ColumnName\"": \""ProductName\"", \""DataType\"": \""System.String\""}],
                \""Rows\"": [
                    {\""ProductId\"": 1, \""ProductName\"": \""Laptop\""},
                    {\""ProductId\"": 2, \""ProductName\"": \""Tablet\""},
                    {\""ProductId\"": 3, \""ProductName\"": \""Smartphone\""},
                    {\""ProductId\"": 4, \""ProductName\"": \""Monitor\""},
                    {\""ProductId\"": 5, \""ProductName\"": \""Keyboard\""}
                ]}"",

            ""{\""Columns\"": [{\""ColumnName\"": \""SalesId\"", \""DataType\"": \""System.Int32\""}, {\""ColumnName\"": \""Amount\"", \""DataType\"": \""System.Decimal\""}],
                \""Rows\"": [
                    {\""SalesId\"": 101, \""Amount\"": 500.75},
                    {\""SalesId\"": 102, \""Amount\"": 300.25},
                    {\""SalesId\"": 103, \""Amount\"": 600.50},
                    {\""SalesId\"": 104, \""Amount\"": 800.00},
                    {\""SalesId\"": 105, \""Amount\"": 450.30}
                ]}""
        ]
        ";


        // Call SplitSerializedAsync to get the deserialized tables
        var tables = await DataTableSerializerWithHooks.SplitSerializedAsync(combinedSerialized);

        int tableIndex = 1;

        //Serialize each dataTable in one file
        foreach (var table in tables)
        {
            string filePath = $"your_absolute_path\\{table}.json";

            Console.WriteLine($"Serializando DataTable {tableIndex} a archivo: {filePath}");
            
            var serializer = new DataTableSerializerWithHooks();
            
            await serializer.SerializeToFileAsync(table, filePath);

            tableIndex++;
        }

        Console.WriteLine("Serialization completed.");
    }
}
````
Output
```
Serializando DataTable 1 a archivo: your_absolute_path\.json
Serializando DataTable 2 a archivo: your_absolute_path\.json
Serialization completed.
```

### Deserialize from file
````csharp
using System.Data;
using NetNinja.Serializers.Implementations.ForDataSets;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = @"your_path\mijson.json";

        var serializer = new DataTableSerializerWithHooks();
        DataTable salesTable = await serializer.DeserializeFromFileAsync(filePath);

        foreach (DataRow row in salesTable.Rows)
        {
            Console.WriteLine($"SalesId: {row["SalesId"]}, Amount: {row["Amount"]}");
        }
    }
}
````
Output
```
Contenido del DataTable:
SalesId: 101, Amount: 500,75
SalesId: 102, Amount: 300,25
SalesId: 103, Amount: 600,5
SalesId: 104, Amount: 800
SalesId: 105, Amount: 450,3
```

### Serialize dynamic data
````csharp
using NetNinja.Serializers.Implementations.ForDynamics;

namespace NetNinja.Serializers.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create test object
            var testObject = new
            {
                Name = "John Doe",
                Age = 30,
                Occupation = "Developer",
                Skills = new[] { "C#", "JavaScript", "SQL" }
            };

            // Create an instance of serializer 
            var serializer = new JsonSerializerForDynamic();

            // Serialize object without indented format
            string serializedData = serializer.Serialize(testObject, indented: false);

            // Print serialization result
            Console.WriteLine("Serialized data without indentation:");
            Console.WriteLine(serializedData);

            // Serialize object with indented format
            string serializedDataIndented = serializer.Serialize(testObject, indented: true);

            //Prinet result of serialization with indentation
            Console.WriteLine("\nSerialized data with indentation:");
            Console.WriteLine(serializedDataIndented);
        }
    }
}
````
Output
```
[Serialization] Object serialized of type: <>f__AnonymousType0`4, Data size: 88 bytes
Serialized data without indentation:
{"Name":"John Doe","Age":30,"Occupation":"Developer","Skills":["C#","JavaScript","SQL"]}
Serialized data with indentation: of type: <>f__AnonymousType0`4, Data size: 132 bytes
{
  "Name": "John Doe",
  "Age": 30,
  "Occupation": "Developer",
  "Skills": [
    "C#",
    "JavaScript",
    "SQL"
  ]
}

```

### Deserialize dynamic data
````csharp
using Newtonsoft.Json;
using NetNinja.Serializers.Implementations.ForDynamics;

namespace NetNinja.Serializers.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string jsonData = @"{
                ""Name"": ""John Doe"",
                ""Age"": 30,
                ""Occupation"": ""Developer"",
                ""Skills"": [""C#"", ""JavaScript"", ""SQL""]
            }";

            var serializer = new JsonSerializerForDynamic();

            var deserializedObject = serializer.Deserialize(jsonData);

            Console.WriteLine("[Deserialization Result]");
            Console.WriteLine($"Type of deserialized object: {deserializedObject.GetType()}");
            Console.WriteLine("Deserialized Data:");

            string prettyJson = JsonConvert.SerializeObject(deserializedObject, Formatting.Indented);
            Console.WriteLine(prettyJson);
        }
    }
}
````
Output
```
[Deserialization] Object deserialized of dynamic type.
[Deserialization Result]
Type of deserialized object: Newtonsoft.Json.Linq.JObject
Deserialized Data:
{
  "Name": "John Doe",
  "Age": 30,
  "Occupation": "Developer",
  "Skills": [
    "C#",
    "JavaScript",
    "SQL"
  ]
}

```

### Bson serialize
Use binary format to reduce the size of operation
````csharp
using NetNinja.Serializers.Implementations.ForHooks;

namespace NetNinja.Serializers.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an example of object to serialize
            var exampleObject = new ExampleObject
            {
                Name = "John Doe",
                Age = 30,
                Skills = new List<string> { "C#", ".NET", "MongoDB" }
            };

            // Create an instance of serializer
            var Bsonserializer = new BsonSerializerWithHooks<ExampleObject>();

            // Serialize in Compact format
            string compactJson = Bsonserializer.Serialize(exampleObject, format: "Compact");
            Console.WriteLine("Serialized data (Compact):");
            Console.WriteLine(compactJson);

            // Serialize in Indented format
            string indentedJson = Bsonserializer.Serialize(exampleObject, format: "Indented");
            Console.WriteLine("\nSerialized data (Indented):");
            Console.WriteLine(indentedJson);
        }
    }

    // sample class
    public class ExampleObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Skills { get; set; }
    }
}
````
Output
```
Serialized data (Compact):
{ "Name" : "John Doe", "Age" : 30, "Skills" : ["C#", ".NET", "MongoDB"] }

Serialized data (Indented):
{
  "Name" : "John Doe",
  "Age" : 30,
  "Skills" : ["C#", ".NET", "MongoDB"]
}
```

### Bson deserialize
````csharp
using NetNinja.Serializers.Implementations.ForHooks;

namespace NetNinja.Serializers.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string bsonData = @"{ ""Name"": ""John Doe"", ""Age"": 30, ""Skills"": [""C#"", "".NET"", ""MongoDB""] }";

            var deserializer = new BsonSerializerWithHooks<ExampleObject>();

            try
            {
                var deserializedObject = deserializer.Deserialize(bsonData);

                Console.WriteLine("[Deserialization Result]");
                Console.WriteLine($"Name: {deserializedObject.Name}");
                Console.WriteLine($"Age: {deserializedObject.Age}");
                Console.WriteLine("Skills: " + string.Join(", ", deserializedObject.Skills));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during deserialization: {ex.Message}");
            }
        }
    }

    public class ExampleObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Skills { get; set; }
    }
}
````
Output
```
[Deserialization Result]
Name: John Doe
Age: 30
Skills: C#, .NET, MongoDB

```

### Bson combine serialized
````csharp
using NetNinja.Serializers.Implementations.ForHooks;

namespace NetNinja.Serializers.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var exampleObjects = new List<ExampleObject>
            {
                new ExampleObject { Name = "John Doe", Age = 30, Skills = new[] { "C#", ".NET", "MongoDB" } },
                new ExampleObject { Name = "Jane Smith", Age = 25, Skills = new[] { "JavaScript", "React", "Node.js" } }
            };

            var serializer = new BsonSerializerWithHooks<ExampleObject>();

            serializer.BeforeSerialize = obj =>
            {
                obj.Name = obj.Name.ToUpper();
                return obj;
            };

            try
            {
                var combinedJson = serializer.CombineSerialized(exampleObjects);

                Console.WriteLine("[Combined JSON Result]");
                Console.WriteLine(combinedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during serialization: {ex.Message}");
            }
        }
    }

    public class ExampleObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Skills { get; set; }
    }
}
````
Output
```
[Combined JSON Result]
[{ "Name" : "JOHN DOE", "Age" : 30, "Skills" : ["C#", ".NET", "MongoDB"] }, { "Name" : "JANE SMITH", "Age" : 25, "Skills" : ["JavaScript", "React", "Node.js"] }]
```

### Bson split serialized
````csharp
using NetNinja.Serializers.Implementations.ForHooks;

namespace NetNinja.Serializers.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string combinedSerialized = @"
            [
                {
                    ""Name"": ""JOHN DOE"",
                    ""Age"": 30,
                    ""Skills"": [""C#"", "".NET"", ""MongoDB""]
                },
                {
                    ""Name"": ""JANE SMITH"",
                    ""Age"": 25,
                    ""Skills"": [""JavaScript"", ""React"", ""Node.js""]
                }
            ]";

            var deserializer = new BsonSerializerWithHooks<ExampleObject>();

            deserializer.AfterDeserialize = obj =>
            {
                obj.Name = obj.Name.ToLower();
                return obj;
            };

            try
            {
                var objects = deserializer.SplitSerialized(combinedSerialized);

                Console.WriteLine("[Split Deserialization Results]");
                foreach (var obj in objects)
                {
                    Console.WriteLine($"Name: {obj.Name}");
                    Console.WriteLine($"Age: {obj.Age}");
                    Console.WriteLine($"Skills: {string.Join(", ", obj.Skills)}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during split deserialization: {ex.Message}");
            }
        }
    }

    public class ExampleObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Skills { get; set; }
    }
}
````
Ouput
```
[Split Deserialization Results]
Name: john doe
Age: 30
Skills: C#, .NET, MongoDB

Name: jane smith
Age: 25
Skills: JavaScript, React, Node.js
```

### Bson async serialize
````csharp
using NetNinja.Serializers.Implementations.ForHooks;

namespace NetNinja.Serializers.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var exampleObject = new ExampleObject
            {
                Name = "John Doe",
                Age = 30,
                Skills = new[] { "C#", ".NET", "MongoDB" }
            };

            var serializer = new BsonSerializerWithHooks<ExampleObject>();

            serializer.BeforeSerialize = obj =>
            {
                obj.Name = obj.Name.ToUpper();
                return obj;
            };

            try
            {
                var compactJson = await serializer.SerializeAsync(exampleObject);
                Console.WriteLine("[Serialized JSON - Compact]");
                Console.WriteLine(compactJson);

                var indentedJson = await serializer.SerializeAsync(exampleObject, "Indented");
                Console.WriteLine("[Serialized JSON - Indented]");
                Console.WriteLine(indentedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during asynchronous serialization: {ex.Message}");
            }
        }
    }

    public class ExampleObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Skills { get; set; }
    }
}
````
Output
```
[Serialized JSON - Compact]
{ "Name" : "JOHN DOE", "Age" : 30, "Skills" : ["C#", ".NET", "MongoDB"] }
[Serialized JSON - Indented]
{
  "Name" : "JOHN DOE",
  "Age" : 30,
  "Skills" : ["C#", ".NET", "MongoDB"]
}

```

### Json serializer with hooks
````csharp
using System;
using Microsoft.Extensions.Logging;
using NetNinja.Serializers.Implementations.ForHooks;

namespace NetNinja.Serializers.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var exampleObject = new ExampleObject
            {
                Name = "John Doe",
                Age = 30,
                Skills = new[] { "C#", ".NET", "MongoDB" }
            };

            /*
             *
             * dotnet add package Microsoft.Extensions.Logging.Console
             */
            
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<JsonSerializerWithHooks<ExampleObject>>();

            var serializer = new JsonSerializerWithHooks<ExampleObject>(
                null, 
                logger,
                null,
                false 
            );

            serializer.BeforeSerialize = obj =>
            {
                obj.Name = obj.Name.ToUpper();
                return obj;
            };

            try
            {
                var compactJson = serializer.Serialize(exampleObject);
                Console.WriteLine("[Serialized JSON - Compact]");
                Console.WriteLine(compactJson);

                var indentedJson = serializer.Serialize(exampleObject, "Indented");
                Console.WriteLine("[Serialized JSON - Indented]");
                Console.WriteLine(indentedJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during serialization: {ex.Message}");
            }
        }
    }

    public class ExampleObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Skills { get; set; }
    }
}
````
Output
```
Serialization started for type: ExampleObject
Serialization completed in 260ms for type: ExampleObject


[Serialized JSON - Compact]
{"Name":"JOHN DOE","Age":30,"Skills":["C#",".NET","MongoDB"]}

Serialization started for type: ExampleObject
Serialization completed in 0ms for type: ExampleObject
Serialized data size: 100 bytes

[Serialized JSON - Indented]
{
  "Name": "JOHN DOE",
  "Age": 30,
  "Skills": [
    "C#",
    ".NET",
    "MongoDB"
  ]
}
```

### Json deserializer with hooks
````csharp
using System.Data;
using Microsoft.Extensions.Logging;
using NetNinja.Serializers.Implementations.ForHooks;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = @"your_absolute_path\mijson.json";

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"El archivo especificado no existe: {filePath}");
            return;
        }

        string jsonData = await File.ReadAllTextAsync(filePath);

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var logger = loggerFactory.CreateLogger<JsonSerializerWithHooks<DataTable>>();

        var serializer = new JsonSerializerWithHooks<DataTable>(
            null, 
            logger,
            null,
            false 
        );

        DataTable salesTable = serializer.Deserialize(jsonData);

        Console.WriteLine("Contenido del DataTable:");
        foreach (DataRow row in salesTable.Rows)
        {
            Console.WriteLine($"SalesId: {row["SalesId"]}, Amount: {row["Amount"]}");
        }
    }
}
````
Ouput
```
Deserialization started for type: DataTable

Deserialization completed in 146ms for type: DataTable

Deserialized data size: 158 bytes

Contenido del DataTable:
SalesId: 101, Amount: 500,75
SalesId: 102, Amount: 300,25
SalesId: 103, Amount: 600,5
SalesId: 104, Amount: 800
SalesId: 105, Amount: 450,3
```

### Json async serializer with hooks
````csharp
using System.Data;
using Microsoft.Extensions.Logging;
using NetNinja.Serializers.Implementations.ForHooks;

class Program
{
    static async Task Main(string[] args)
    {
        var salesTable = new DataTable("Sales");
        salesTable.Columns.Add("SalesId", typeof(int));
        salesTable.Columns.Add("Amount", typeof(decimal));

        salesTable.Rows.Add(101, 500.75m);
        salesTable.Rows.Add(102, 300.25m);
        salesTable.Rows.Add(103, 600.5m);
        salesTable.Rows.Add(104, 800.0m);
        salesTable.Rows.Add(105, 450.3m);

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var logger = loggerFactory.CreateLogger<JsonSerializerWithHooks<DataTable>>();

        var serializer = new JsonSerializerWithHooks<DataTable>(
            null, 
            logger,
            null,
            false 
        );

        serializer.BeforeSerialize = dataTable =>
        {
            dataTable.Columns.Add("ProcessedAt", typeof(DateTime));
            foreach (DataRow row in dataTable.Rows)
            {
                row["ProcessedAt"] = DateTime.Now;
            }
            return dataTable;
        };

        string jsonResult = await serializer.SerializeAsync(salesTable, format: "Indented");

        Console.WriteLine("Resultado de la serialización:");
        Console.WriteLine(jsonResult);
    }
}
````
Ouput
```
Result of serialization:
[
  {
    "SalesId": 101,
    "Amount": 500.75,
    "ProcessedAt": "2024-12-21T02:18:32.1587808"
  },
  {
    "SalesId": 102,
    "Amount": 300.25,
    "ProcessedAt": "2024-12-21T02:18:32.2042459"
  },
  {
    "SalesId": 103,
    "Amount": 600.5,
    "ProcessedAt": "2024-12-21T02:18:32.2043239"
  },
  {
    "SalesId": 104,
    "Amount": 800.0,
    "ProcessedAt": "2024-12-21T02:18:32.2043316"
  },
  {
    "SalesId": 105,
    "Amount": 450.3,
    "ProcessedAt": "2024-12-21T02:18:32.2043355"
  }
]
```

### Serialize xml with hooks
````csharp
using NetNinja.Serializers.Implementations.ForHooks;

class Program
{
    static void Main()
    {
        var person = new Person
        {
            Id = 1,
            Name = "John Doe",
            BirthDate = new DateTime(1990, 5, 1)
        };

        var serializer = new XmlSerializerWithHooks<Person>();

        serializer.BeforeSerialize = p =>
        {
            Console.WriteLine("Modificando el objeto antes de serializar...");
            p.Name = p.Name.ToUpper(); 
            return p;
        };

        string xml = serializer.Serialize(person, format: "Indented");

        Console.WriteLine("Resultado de la serialización XML:");
        Console.WriteLine(xml);
    }
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
}
````
Ouput
```
Modificando el objeto antes de serializar...
Resultado de la serialización XML:
<?xml version="1.0" encoding="utf-16"?>
<Person xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Id>1</Id>
  <Name>JOHN DOE</Name>
  <BirthDate>1990-05-01T00:00:00</BirthDate>
</Person>
```

### Derialize xml with hooks
````csharp
using NetNinja.Serializers.Implementations.ForHooks;

class Program
{
    static void Main()
    {
        string xmlData = @"<?xml version=""1.0"" encoding=""utf-8""?>
        <Person>
          <Id>1</Id>
          <Name>JOHN DOE</Name>
          <BirthDate>1990-05-01T00:00:00</BirthDate>
        </Person>";

        var serializer = new XmlSerializerWithHooks<Person>();

        serializer.AfterDeserialize = obj =>
        {
            Console.WriteLine("Transformando el objeto después de deserializar...");
            obj.Name = obj.Name.ToLower();
            return obj;
        };

        var person = serializer.Deserialize(xmlData);

        Console.WriteLine("Objeto deserializado:");
        Console.WriteLine($"Id: {person.Id}");
        Console.WriteLine($"Name: {person.Name}");
        Console.WriteLine($"BirthDate: {person.BirthDate}");
    }
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
}
````
Ouput
```
Transformando el objeto después de deserializar...
Objeto deserializado:
Id: 1
Name: john doe
BirthDate: 01/05/1990 0:00:00
```

### Serialize yaml with hooks
````csharp
using NetNinja.Serializers.Implementations.ForHooks;

class Program
{
    static void Main()
    {
        var person = new Person
        {
            Id = 1,
            Name = "John Doe",
            BirthDate = new DateTime(1990, 5, 1),
            IsActive = true
        };

        var serializer = new YamlSerializerWithHooks<Person>();

        serializer.BeforeSerialize = obj =>
        {
            Console.WriteLine("Modificando el objeto antes de serializar...");
            obj.Name = obj.Name.ToUpper(); 
            return obj;
        };

        string yaml = serializer.Serialize(person, format: "Indented");

        Console.WriteLine("Resultado de la serialización YAML:");
        Console.WriteLine(yaml);
    }
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
}
````
ouput
```
Modificando el objeto antes de serializar...
Resultado de la serialización YAML:
Id: 1
Name: JOHN DOE
BirthDate: 1990-05-01T00:00:00.0000000
IsActive: true
```

### Deserialize yaml with hooks
````csharp
using NetNinja.Serializers.Implementations.ForHooks;

class Program
{
    static void Main()
    {
        string yamlData = @"
        Id: 1
        Name: JOHN DOE
        BirthDate: 1990-05-01T00:00:00
        IsActive: true";

        var serializer = new YamlSerializerWithHooks<Person>();

        serializer.AfterDeserialize = obj =>
        {
            Console.WriteLine("Modificando el objeto después de deserializar...");
            obj.Name = obj.Name.ToLower(); 
            return obj;
        };

        var person = serializer.Deserialize(yamlData);

        Console.WriteLine("Objeto deserializado:");
        Console.WriteLine($"Id: {person.Id}");
        Console.WriteLine($"Name: {person.Name}");
        Console.WriteLine($"BirthDate: {person.BirthDate}");
        Console.WriteLine($"IsActive: {person.IsActive}");
    }
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
}
````
Ouput
```
Modificando el objeto después de deserializar...
Objeto deserializado:
Id: 1
Name: john doe
BirthDate: 01/05/1990 0:00:00
IsActive: True
```

### Serialize message from network
````csharp
using Microsoft.Extensions.Logging;
using NetNinja.Serializers.Implementations.ForHooks;
using NetNinja.Serializers.Implementations.Network;

namespace NetNinja.Serializers.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var exampleObject = new ExampleObject
            {
                Name = "John Doe",
                Age = 30,
                Skills = new[] { "C#", ".NET", "MongoDB" }
            };

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<JsonSerializerWithHooks<ExampleObject>>();

            var jsonSerializer = new JsonSerializerWithHooks<ExampleObject>(
                null, 
                logger,
                null, 
                false 
            );

            var networkSerializer = new NetworkSerializer<ExampleObject>(jsonSerializer);

            var host = "127.0.0.1"; 
            var port = 12345;       
            
            try
            {
                networkSerializer.SerializeToNetwork(exampleObject, host, port);
                Console.WriteLine("Data serialized and sent to network successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during SerializeToNetwork: {ex.Message}");
            }
        }
    }

    public class ExampleObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Skills { get; set; }
    }
}
````
Output

```
Serialized data size: 61 bytes
Error during SerializeToNetwork: No se puede establecer una conexión ya que el equipo de destino denegó expresamente dicha conexión. [::ffff:127.0.0.1]:12345
```

### Deserialize message from network
````csharp
using Microsoft.Extensions.Logging;
using NetNinja.Serializers.Implementations.ForHooks;
using NetNinja.Serializers.Implementations.Network;

namespace TcpClientExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "127.0.0.1"; 
            var port = 12345;      

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<JsonSerializerWithHooks<ExampleObject>>();

            var jsonSerializer = new JsonSerializerWithHooks<ExampleObject>(
                null, 
                logger,
                null, 
                false 
            );

            var networkSerializer = new NetworkSerializer<ExampleObject>(jsonSerializer);

            var exampleObject = new ExampleObject
            {
                Name = "Client Data",
                Age = 30,
                Skills = new[] { "C#", ".NET", "JSON" }
            };

            Console.WriteLine("Client is sending data to server...");
            networkSerializer.SerializeToNetwork(exampleObject, host, port);
            Console.WriteLine("Data sent!");

            Console.WriteLine("Client is waiting for response from server...");
            var response = networkSerializer.DeserializeFromNetwork(host, port);

            Console.WriteLine("Response received and deserialized:");
            Console.WriteLine($"Name: {response.Name}");
            Console.WriteLine($"Age: {response.Age}");
            Console.WriteLine($"Skills: {string.Join(", ", response.Skills)}");
        }
    }

    public class ExampleObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Skills { get; set; }
    }
}
````

### Beson Serialize and deserialize
methods : SerializeToBson(), Serialize(), SerializeToStream(), SerializeMessages(), DeserializeFromBson(), Deserialize(), DeserializeFromStream(), SerializeCompressed(), DeserializeCompressed(), ValidateSerializedData()
````csharp
using NetNinja.Serializers.Implementations.Regular;

class Program
{
    static void Main(string[] args)
    {
        var serializer = new BsonSerializer<ExampleObject>();

        var exampleObject = new ExampleObject
        {
            Name = "Example User",
            Age = 35,
            Skills = new[] { "C#", "BSON", "Serialization" }
        };

        Console.WriteLine("=== Serialization Methods ===");

        var bsonBytes = serializer.SerializeToBson(exampleObject);
        Console.WriteLine($"SerializeToBson: {BitConverter.ToString(bsonBytes)}");

        var hexString = serializer.Serialize(exampleObject);
        Console.WriteLine($"Serialize: {hexString}");

        using (var memoryStream = new MemoryStream())
        {
            serializer.SerializeToStream(exampleObject, memoryStream);
            Console.WriteLine($"SerializeToStream -> Stream Length: {memoryStream.Length}");
        }

        var objectList = new List<ExampleObject>
        {
            new ExampleObject { Name = "User 1", Age = 25, Skills = new[] { "Skill1" } },
            new ExampleObject { Name = "User 2", Age = 30, Skills = new[] { "Skill2", "Skill3" } }
        };

        var serializedMessages = serializer.SerializeMessages(objectList);
        Console.WriteLine($"SerializeMessages -> Hex String: {serializedMessages}");

        Console.WriteLine("\n=== Deserialization Methods ===");

        var deserializedFromBson = serializer.DeserializeFromBson(bsonBytes);
        Console.WriteLine($"DeserializeFromBson: Name={deserializedFromBson.Name}, Age={deserializedFromBson.Age}");

        var deserializedFromHex = serializer.Deserialize(hexString);
        Console.WriteLine($"Deserialize: Name={deserializedFromHex.Name}, Age={deserializedFromHex.Age}");

        using (var stream = new MemoryStream(bsonBytes))
        {
            var deserializedFromStream = serializer.DeserializeFromStream(stream);
            Console.WriteLine($"DeserializeFromStream: Name={deserializedFromStream.Name}, Age={deserializedFromStream.Age}");
        }

        Console.WriteLine("\n=== Compressed Methods ===");

        var compressedData = serializer.SerializeCompressed(exampleObject);
        Console.WriteLine($"SerializeCompressed -> Compressed Size: {compressedData.Length} bytes");

        var decompressedObject = serializer.DeserializeCompressed(compressedData);
        Console.WriteLine($"DeserializeCompressed: Name={decompressedObject.Name}, Age={decompressedObject.Age}");

        Console.WriteLine("\n=== Validation Methods ===");

        const string schema = @"
        {
            'type': 'object',
            'properties': {
                'Name': { 'type': 'string' },
                'Age': { 'type': 'integer' },
                'Skills': {
                    'type': 'array',
                    'items': { 'type': 'string' }
                }
            },
            'required': ['Name', 'Age']
        }";

        var isValid = serializer.ValidateSerializedData(hexString, schema);
        Console.WriteLine($"ValidateSerializedData: Is Valid? {isValid}");
    }
}

public class ExampleObject
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string[] Skills { get; set; }
}
````
Output
```
=== Serialization Methods ===
SerializeToBson: 5D-00-00-00-02-4E-61-6D-65-00-0D-00-00-00-45-78-61-6D-70-6C-65-20-55-73-65-72-00-10-41-67-65-00-23-00-00-00-04-53-6B-69-6C-6C-73-00-30-00-00-00-02-30-00-03-00-00-00-43-23-00-02-31-00-05-00-00-00-42-53-4F-4E-00-02-32-00-0E-00-00-00-53-65-72-69-61-6C-69-7A-61-74-69-6F-6E-00-00-00
Serialize: 5d000000024e616d65000d0000004578616d706c6520557365720010416765002300000004536b696c6c730030000000023000030000004323000231000500000042534f4e000232000e00000053657269616c697a6174696f6e000000
SerializeToStream -> Stream Length: 93
SerializeMessages -> Hex String: 9c000000044d65737361676573008d0000000330003a000000024e616d6500070000005573657220310010416765001900000004536b696c6c73001300000002300007000000536b696c6c3100000003310048000000024e616d6500070000005573657220320010416765001e00000004536b696c6c73002100000002300007000000536b696c6c320002310007000000536b696c6c330000000000

=== Deserialization Methods ===
DeserializeFromBson: Name=Example User, Age=35
Deserialize: Name=Example User, Age=35
DeserializeFromStream: Name=Example User, Age=35

=== Compressed Methods ===
SerializeCompressed -> Compressed Size: 103 bytes
=== Validation Methods ===e=Example User, Age=35
BSON Validation Error: Invalid JSON number '5d'.
ValidateSerializedData: Is Valid? False
```

### NewtonSoft Serialize and deserialize
methods = Serialize(), SerializeMessages(), SerializeToStream(), Deserialize(), DeserializeFromStream(), SerializeCompressed(), DeserializeCompressed(), ValidateSerializedData()
````csharp
using System.Text;
using NetNinja.Serializers.Implementations.Regular;

class Program
{
    static void Main()
    {
        var serializer = new NewtonsoftSerializer<ExampleObject>();

        var exampleObject = new ExampleObject
        {
            Name = "Example User",
            Age = 35,
            Skills = new[] { "C#", "JSON", "Serialization" }
        };

        Console.WriteLine("=== Serialization Methods ===");

        var jsonString = serializer.Serialize(exampleObject);
        Console.WriteLine($"Serialize -> JSON String: {jsonString}");

        var objectList = new List<ExampleObject>
        {
            new ExampleObject { Name = "User 1", Age = 25, Skills = new[] { "Skill1" } },
            new ExampleObject { Name = "User 2", Age = 30, Skills = new[] { "Skill2", "Skill3" } }
        };
        var jsonMessages = serializer.SerializeMessages(objectList);
        Console.WriteLine($"SerializeMessages -> JSON: {jsonMessages}");

        using (var memoryStream = new MemoryStream())
        {
            serializer.SerializeToStream(exampleObject, memoryStream);
            var serializedData = Encoding.UTF8.GetString(memoryStream.ToArray());
            Console.WriteLine($"SerializeToStream -> JSON in Stream: {serializedData}");
        }

        Console.WriteLine("\n=== Deserialization Methods ===");

        var deserializedObject = serializer.Deserialize(jsonString);
        Console.WriteLine($"Deserialize -> Name: {deserializedObject.Name}, Age: {deserializedObject.Age}");

        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
        {
            var deserializedFromStream = serializer.DeserializeFromStream(memoryStream);
            Console.WriteLine($"DeserializeFromStream -> Name: {deserializedFromStream.Name}, Age: {deserializedFromStream.Age}");
        }

        Console.WriteLine("\n=== Compressed Methods ===");

        var compressedData = serializer.SerializeCompressed(exampleObject);
        Console.WriteLine($"SerializeCompressed -> Compressed Size: {compressedData.Length} bytes");

        var decompressedObject = serializer.DeserializeCompressed(compressedData);
        Console.WriteLine($"DeserializeCompressed -> Name: {decompressedObject.Name}, Age: {decompressedObject.Age}");

        Console.WriteLine("\n=== Validation Methods ===");

        const string schema = @"
        {
            'type': 'object',
            'properties': {
                'Name': { 'type': 'string' },
                'Age': { 'type': 'integer' },
                'Skills': {
                    'type': 'array',
                    'items': { 'type': 'string' }
                }
            },
            'required': ['Name', 'Age']
        }";

        var isValid = serializer.ValidateSerializedData(jsonString, schema);
        Console.WriteLine($"ValidateSerializedData -> Is Valid? {isValid}");
    }
}

public class ExampleObject
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string[] Skills { get; set; }
}
````
Output
```
=== Serialization Methods ===
Serialize -> JSON String: {"Name":"Example User","Age":35,"Skills":["C#","JSON","Serialization"]}
SerializeMessages -> JSON: [{"Name":"User 1","Age":25,"Skills":["Skill1"]},{"Name":"User 2","Age":30,"Skills":["Skill2","Skill3"]}]
SerializeToStream -> JSON in Stream: {"Name":"Example User","Age":35,"Skills":["C#","JSON","Serialization"]}

=== Deserialization Methods ===
Deserialize -> Name: Example User, Age: 35
DeserializeFromStream -> Name: Example User, Age: 35

=== Compressed Methods ===
SerializeCompressed -> Compressed Size: 87 bytes
DeserializeCompressed -> Name: Example User, Age: 35

=== Validation Methods ===
ValidateSerializedData -> Is Valid? True
```

### SystemText Serialize and deserialize
methods : Serialize(), SerializeMessages(), SerializeToStream(), Deserialize(), DeserializeFromStream(), SerializeCompressed(), DeserializeCompressed(), ValidateSerializedData()
````csharp
using System.Text;
using NetNinja.Serializers.Implementations.Regular;

class Program
{
    static void Main()
    {
        var serializer = new SystemTextSerializer<ExampleObject>();

        var exampleObject = new ExampleObject
        {
            Name = "Example User",
            Age = 35,
            Skills = new[] { "C#", "System.Text.Json", "Serialization" }
        };

        Console.WriteLine("=== Serialization Methods ===");

        var jsonString = serializer.Serialize(exampleObject);
        Console.WriteLine($"Serialize -> JSON String: {jsonString}");

        var objectList = new List<ExampleObject>
        {
            new ExampleObject { Name = "User 1", Age = 25, Skills = new[] { "Skill1" } },
            new ExampleObject { Name = "User 2", Age = 30, Skills = new[] { "Skill2", "Skill3" } }
        };
        var jsonMessages = serializer.SerializeMessages(objectList);
        Console.WriteLine($"SerializeMessages -> JSON: {jsonMessages}");

        using (var memoryStream = new MemoryStream())
        {
            serializer.SerializeToStream(exampleObject, memoryStream);
            var serializedData = Encoding.UTF8.GetString(memoryStream.ToArray());
            Console.WriteLine($"SerializeToStream -> JSON in Stream: {serializedData}");
        }

        Console.WriteLine("\n=== Deserialization Methods ===");

        var deserializedObject = serializer.Deserialize(jsonString);
        Console.WriteLine($"Deserialize -> Name: {deserializedObject.Name}, Age: {deserializedObject.Age}");

        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
        {
            var deserializedFromStream = serializer.DeserializeFromStream(memoryStream);
            Console.WriteLine($"DeserializeFromStream -> Name: {deserializedFromStream.Name}, Age: {deserializedFromStream.Age}");
        }

        Console.WriteLine("\n=== Compressed Methods ===");

        var compressedData = serializer.SerializeCompressed(exampleObject);
        Console.WriteLine($"SerializeCompressed -> Compressed Size: {compressedData.Length} bytes");

        var decompressedObject = serializer.DeserializeCompressed(compressedData);
        Console.WriteLine($"DeserializeCompressed -> Name: {decompressedObject.Name}, Age: {decompressedObject.Age}");

        Console.WriteLine("\n=== Validation Methods ===");

        const string schema = @"
        {
            'type': 'object',
            'properties': {
                'Name': { 'type': 'string' },
                'Age': { 'type': 'integer' },
                'Skills': {
                    'type': 'array',
                    'items': { 'type': 'string' }
                }
            },
            'required': ['Name', 'Age']
        }";

        var isValid = serializer.ValidateSerializedData(jsonString, schema);
        Console.WriteLine($"ValidateSerializedData -> Is Valid? {isValid}");
    }
}

public class ExampleObject
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string[] Skills { get; set; }
}
````
Output
```
=== Serialization Methods ===
Serialize -> JSON String: {"Name":"Example User","Age":35,"Skills":["C#","System.Text.Json","Serialization"]}
SerializeMessages -> JSON: [{"Name":"User 1","Age":25,"Skills":["Skill1"]},{"Name":"User 2","Age":30,"Skills":["Skill2","Skill3"]}]
SerializeToStream -> JSON in Stream: {"Name":"Example User","Age":35,"Skills":["C#","System.Text.Json","Serialization"]}

=== Deserialization Methods ===
Deserialize -> Name: Example User, Age: 35
DeserializeFromStream -> Name: Example User, Age: 35

=== Compressed Methods ===
SerializeCompressed -> Compressed Size: 97 bytes
DeserializeCompressed -> Name: Example User, Age: 35

=== Validation Methods ===
ValidateSerializedData -> Is Valid? True
```

### Xml Serialize and deserialize
methods : Serialize(), SerializeMessages(), SerializeToStream(), Deserialize(), DeserializeFromStream(), SerializeCompressed(), DeserializeCompressed(), ValidateSerializedData()
````csharp
using System.Text;
using NetNinja.Serializers.Implementations.Regular;

class Program
{
    static void Main()
    {
        var serializer = new XmlSerializer<ExampleObject>();

        var exampleObject = new ExampleObject
        {
            Name = "Example User",
            Age = 35,
            Skills = new[] { "C#", "XML", "Serialization" }
        };

        Console.WriteLine("=== Serialization Methods ===");

        var xmlString = serializer.Serialize(exampleObject);
        Console.WriteLine($"Serialize -> XML String:\n{xmlString}");

        var objectList = new List<ExampleObject>
        {
            new ExampleObject { Name = "User 1", Age = 25, Skills = new[] { "Skill1" } },
            new ExampleObject { Name = "User 2", Age = 30, Skills = new[] { "Skill2", "Skill3" } }
        };
        var xmlMessages = serializer.SerializeMessages(objectList);
        Console.WriteLine($"SerializeMessages -> XML:\n{xmlMessages}");

        using (var memoryStream = new MemoryStream())
        {
            serializer.SerializeToStream(exampleObject, memoryStream);
            var serializedData = Encoding.UTF8.GetString(memoryStream.ToArray());
            Console.WriteLine($"SerializeToStream -> XML in Stream:\n{serializedData}");
        }

        Console.WriteLine("\n=== Deserialization Methods ===");

        var deserializedObject = serializer.Deserialize(xmlString);
        Console.WriteLine($"Deserialize -> Name: {deserializedObject.Name}, Age: {deserializedObject.Age}");

        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
        {
            var deserializedFromStream = serializer.DeserializeFromStream(memoryStream);
            Console.WriteLine($"DeserializeFromStream -> Name: {deserializedFromStream.Name}, Age: {deserializedFromStream.Age}");
        }

        Console.WriteLine("\n=== Compressed Methods ===");

        var compressedData = serializer.SerializeCompressed(exampleObject);
        Console.WriteLine($"SerializeCompressed -> Compressed Size: {compressedData.Length} bytes");

        var decompressedObject = serializer.DeserializeCompressed(compressedData);
        Console.WriteLine($"DeserializeCompressed -> Name: {decompressedObject.Name}, Age: {decompressedObject.Age}");

        Console.WriteLine("\n=== Validation Methods ===");

        const string schema = @"
        <xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>
            <xs:element name='ExampleObject'>
                <xs:complexType>
                    <xs:sequence>
                        <xs:element name='Name' type='xs:string'/>
                        <xs:element name='Age' type='xs:int'/>
                        <xs:element name='Skills'>
                            <xs:complexType>
                                <xs:sequence>
                                    <xs:element name='Skill' type='xs:string' maxOccurs='unbounded'/>
                                </xs:sequence>
                            </xs:complexType>
                        </xs:element>
                    </xs:sequence>
                </xs:complexType>
            </xs:element>
        </xs:schema>";

        var isValid = serializer.ValidateSerializedData(xmlString, schema);
        Console.WriteLine($"ValidateSerializedData -> Is Valid? {isValid}");
    }
}

public class ExampleObject
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string[] Skills { get; set; }
}
````
Output
```
=== Serialization Methods ===
Serialize -> XML String:
<?xml version="1.0" encoding="utf-16"?>
<ExampleObject xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Name>Example User</Name>
  <Age>35</Age>
  <Skills>
    <string>C#</string>
    <string>XML</string>
    <string>Serialization</string>
  </Skills>
</ExampleObject>
SerializeMessages -> XML:
<?xml version="1.0" encoding="utf-16"?>
<ArrayOfExampleObject xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ExampleObject>
    <Name>User 1</Name>
    <Age>25</Age>
    <Skills>
      <string>Skill1</string>
    </Skills>
  </ExampleObject>
  <ExampleObject>
    <Name>User 2</Name>
    <Age>30</Age>
    <Skills>
      <string>Skill2</string>
      <string>Skill3</string>
    </Skills>
  </ExampleObject>
</ArrayOfExampleObject>
SerializeToStream -> XML in Stream:
<?xml version="1.0" encoding="utf-8"?>
<ExampleObject xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="
                                                                               "http://www.w3.org/2001/XMLSchema">
  <Name>Example User</Name>
  <Age>35</Age>
  <Skills>
    <string>C#</string>
    <string>XML</string>
    <string>Serialization</string>
  </Skills>
</ExampleObject>

=== Deserialization Methods ===
Deserialize -> Name: Example User, Age: 35
DeserializeFromStream -> Name: Example User, Age: 35

=== Compressed Methods ===
SerializeCompressed -> Compressed Size: 210 bytes
DeserializeCompressed -> Name: Example User, Age: 35

=== Validation Methods ===
XML Validation Error: Validation failed: The element 'Skills' has invalid child 
                                                                                element 'string'. List of possible elements expected: 'Skill'.
ValidateSerializedData -> Is Valid? False
```

### Yaml Serialize and deserialize
methods : Serialize(), SerializeMessages(), SerializeToStream(), Deserialize(), DeserializeFromStream(), SerializeCompressed(), DeserializeCompressed() , ValidateSerializedData()
````csharp
using System.Text;
using NetNinja.Serializers.Implementations.Regular;

class Program
{
    static void Main()
    {
        var serializer = new YamlSerializer<ExampleObject>();

        var exampleObject = new ExampleObject
        {
            Name = "Example User",
            Age = 35,
            Skills = new[] { "C#", "YAML", "Serialization" }
        };

        Console.WriteLine("=== Serialization Methods ===");

        var yamlString = serializer.Serialize(exampleObject);
        Console.WriteLine($"Serialize -> YAML String:\n{yamlString}");

        var objectList = new List<ExampleObject>
        {
            new ExampleObject { Name = "User 1", Age = 25, Skills = new[] { "Skill1" } },
            new ExampleObject { Name = "User 2", Age = 30, Skills = new[] { "Skill2", "Skill3" } }
        };
        var yamlMessages = serializer.SerializeMessages(objectList);
        Console.WriteLine($"SerializeMessages -> YAML:\n{yamlMessages}");

        using (var memoryStream = new MemoryStream())
        {
            serializer.SerializeToStream(exampleObject, memoryStream);
            var serializedData = Encoding.UTF8.GetString(memoryStream.ToArray());
            Console.WriteLine($"SerializeToStream -> YAML in Stream:\n{serializedData}");
        }

        Console.WriteLine("\n=== Deserialization Methods ===");

        var deserializedObject = serializer.Deserialize(yamlString);
        Console.WriteLine($"Deserialize -> Name: {deserializedObject.Name}, Age: {deserializedObject.Age}");

        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(yamlString)))
        {
            var deserializedFromStream = serializer.DeserializeFromStream(memoryStream);
            Console.WriteLine($"DeserializeFromStream -> Name: {deserializedFromStream.Name}, Age: {deserializedFromStream.Age}");
        }

        Console.WriteLine("\n=== Compressed Methods ===");

        var compressedData = serializer.SerializeCompressed(exampleObject);
        Console.WriteLine($"SerializeCompressed -> Compressed Size: {compressedData.Length} bytes");

        var decompressedObject = serializer.DeserializeCompressed(compressedData);
        Console.WriteLine($"DeserializeCompressed -> Name: {decompressedObject.Name}, Age: {decompressedObject.Age}");

        Console.WriteLine("\n=== Validation Methods ===");

        const string schema = @"
        {
            'type': 'object',
            'properties': {
                'Name': { 'type': 'string' },
                'Age': { 'type': 'integer' },
                'Skills': {
                    'type': 'array',
                    'items': { 'type': 'string' }
                }
            },
            'required': ['Name', 'Age']
        }";

        var isValid = serializer.ValidateSerializedData(yamlString, schema);
        Console.WriteLine($"ValidateSerializedData -> Is Valid? {isValid}");
    }
}

public class ExampleObject
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string[] Skills { get; set; }
}
````
Output
```
=== Serialization Methods ===
Serialize -> YAML String:
name: Example User
age: 35
skills:
- C#
- YAML
- Serialization

SerializeMessages -> YAML:
- name: User 1
  age: 25
  skills:
  - Skill1
- name: User 2
  age: 30
  skills:
  - Skill2
  - Skill3

SerializeToStream -> YAML in Stream:
name: Example User
age: 35
skills:
- C#
- YAML
- Serialization


=== Deserialization Methods ===
Deserialize -> Name: Example User, Age: 35
DeserializeFromStream -> Name: Example User, Age: 35

=== Compressed Methods ===
SerializeCompressed -> Compressed Size: 83 bytes
DeserializeCompressed -> Name: Example User, Age: 35

=== Validation Methods ===
ValidateSerializedData -> Is Valid? False
```

### Beson versioned serialize and deserialize
Methods : Serialize(), SerializeMessages(), SerializeToStream(), Deserialize(), SerializeToStream(), DeserializeFromStream(), SerializeCompressed(), SerializeWithVersion(), SerializeWithVersion(), DeserializeWithVersion(), ValidateSerializedData()
````csharp
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Implementations.Versioned;

class Program
{
    static void Main()
    {
        var serializer = new BsonSerializerWithVersion<ExampleObject>();

        var exampleObject = new ExampleObject
        {
            Name = "Example User",
            Age = 35,
            Skills = new[] { "C#", "BSON", "Serialization" },
            Version = "1.0"
        };

        Console.WriteLine("=== Serialization Methods ===");

        var bsonString = serializer.Serialize(exampleObject);
        Console.WriteLine($"Serialize -> BSON String:\n{bsonString}");

        var objectList = new List<ExampleObject>
        {
            new ExampleObject { Name = "User 1", Age = 25, Skills = new[] { "Skill1" }, Version = "1.0" },
            new ExampleObject { Name = "User 2", Age = 30, Skills = new[] { "Skill2", "Skill3" }, Version = "1.1" }
        };
        var bsonMessages = serializer.SerializeMessages(objectList);
        Console.WriteLine($"SerializeMessages -> BSON Array:\n{bsonMessages}");

        using (var memoryStream = new MemoryStream())
        {
            serializer.SerializeToStream(exampleObject, memoryStream);
            var serializedData = Convert.ToBase64String(memoryStream.ToArray());
            Console.WriteLine($"SerializeToStream -> Serialized Base64 in Stream:\n{serializedData}");
        }

        Console.WriteLine("\n=== Deserialization Methods ===");

        var deserializedObject = serializer.Deserialize(bsonString);
        Console.WriteLine($"Deserialize -> Name: {deserializedObject.Name}, Version: {deserializedObject.Version}");

        using (var memoryStream = new MemoryStream())
        {
            serializer.SerializeToStream(exampleObject, memoryStream);
            memoryStream.Position = 0; 
            var deserializedFromStream = serializer.DeserializeFromStream(memoryStream);
            Console.WriteLine($"DeserializeFromStream -> Name: {deserializedFromStream.Name}, Version: {deserializedFromStream.Version}");
        }

        Console.WriteLine("\n=== Compressed Methods ===");

        var compressedData = serializer.SerializeCompressed(exampleObject);
        Console.WriteLine($"SerializeCompressed -> Compressed Size: {compressedData.Length} bytes");

        var decompressedObject = serializer.DeserializeCompressed(compressedData);
        Console.WriteLine($"DeserializeCompressed -> Name: {decompressedObject.Name}, Version: {decompressedObject.Version}");

        Console.WriteLine("\n=== Versioned Methods ===");

        var bsonWithVersion = serializer.SerializeWithVersion(exampleObject, "2.0");
        Console.WriteLine($"SerializeWithVersion -> BSON Document with Version:\n{bsonWithVersion}");

        var (deserializedObjWithVersion, version) = serializer.DeserializeWithVersion(bsonWithVersion);
        Console.WriteLine($"DeserializeWithVersion -> Name: {deserializedObjWithVersion.Name}, Retrieved Version: {version}");

        Console.WriteLine("\n=== Validation Methods ===");

        var isValid = serializer.ValidateSerializedData(bsonString, string.Empty);
        Console.WriteLine($"ValidateSerializedData -> Is Valid? {isValid}");
    }
}

public class ExampleObject : IVersioned
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string[] Skills { get; set; }
    public string Version { get; set; }
}
````
Ouput
```
=== Deserialization Methods ===
Deserialize -> Name: Example User, Version: 1.0
DeserializeFromStream -> Name: Example User, Version: 1.0

=== Compressed Methods ===
SerializeCompressed -> Compressed Size: 116 bytes
=== Versioned Methods ===Name: Example User, Version: 1.0
SerializeWithVersion -> BSON Document with Version:
{ "Name" : "Example User", "Age" : 35, "Skills" : ["C#", "BSON", "Serialization"], "Version" : "2.0" }
DeserializeWithVersion -> Name: Example User, Retrieved Version: 2.0

=== Validation Methods ===
ValidateSerializedData -> Is Valid? True
```

### Newtonsoft versioned serialize and deserialize
Methods : Serialize(), Deserialize(), SerializeCompressed(), DeserializeCompressed(), SerializeWithVersion(), DeserializeWithVersion(), ValidateSerializedData(), SerializeMessages()
````csharp
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Implementations.Versioned;

namespace ExampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var person = new Person
            {
                Id = 1,
                Name = "John Doe",
                BirthDate = new DateTime(1990, 5, 1),
                IsActive = true,
                Version = "1.0"
            };

            var serializer = new NewtonsoftSerializerWithVersion<Person>();

            Console.WriteLine("== Serialización ==");
            var serializedData = serializer.Serialize(person);
            Console.WriteLine($"Serialized Data:\n{serializedData}");

            Console.WriteLine("\n== Deserialización Simple ==");
            var deserializedPerson = serializer.Deserialize(serializedData);
            Console.WriteLine($"Deserialized Person:\nID: {deserializedPerson.Id}\nName: {deserializedPerson.Name}");

            Console.WriteLine("\n== Serialización Comprimida ==");
            var compressedData = serializer.SerializeCompressed(person);
            Console.WriteLine($"Compressed Data (byte array length): {compressedData.Length}");

            Console.WriteLine("\n== Deserialización Comprimida ==");
            var decompressedPerson = serializer.DeserializeCompressed(compressedData);
            Console.WriteLine($"Decompressed Person:\nID: {decompressedPerson.Id}\nName: {decompressedPerson.Name}");

            Console.WriteLine("\n== Serialización con Versión ==");
            var serializedWithVersion = serializer.SerializeWithVersion(person, "2.0");
            Console.WriteLine($"Serialized Data with Version:\n{serializedWithVersion}");

            Console.WriteLine("\n== Deserialización con Versión ==");
            var (deserializedWithVersion, version) = serializer.DeserializeWithVersion(serializedWithVersion);
            Console.WriteLine($"Version Extracted: {version}");
            Console.WriteLine($"Deserialized Object (With Version):\nID: {deserializedWithVersion.Id}\nName: {deserializedWithVersion.Name}");

            Console.WriteLine("\n== Validación de Datos Serializados ==");
            var isValid = serializer.ValidateSerializedData(serializedData, null);
            Console.WriteLine($"Is Serialized Data Valid? {isValid}");

            Console.WriteLine("\n== Serialización de Lista ==");
            var people = new List<Person>
            {
                new Person { Id = 2, Name = "Jane Doe", BirthDate = new DateTime(1995, 7, 15), IsActive = false },
                new Person { Id = 3, Name = "Jack Smith", BirthDate = new DateTime(1988, 3, 10), IsActive = true }
            };
            var serializedMessages = serializer.SerializeMessages(people);
            Console.WriteLine($"Serialized List:\n{serializedMessages}");
        }    
    }

    public class Person : IVersioned
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
        public string Version { get; set; }
    }
}
````
Output
```
== Serialización ==
Serialized Data:
{
  "Id": 1,
  "Name": "John Doe",
  "BirthDate": "1990-05-01T00:00:00",
  "IsActive": true,
  "Version": "1.0"
}

== Deserialización Simple ==
Deserialized Person:
ID: 1
Name: John Doe

== Serialización Comprimida ==
Compressed Data (byte array length): 109

== Deserialización Comprimida ==
Decompressed Person:
ID: 1
== Serialización con Versión ==
Serialized Data with Version:
{
  "Id": 1,
  "Name": "John Doe",
  "BirthDate": "1990-05-01T00:00:00",
  "IsActive": true,
  "Version": "2.0"
}

== Deserialización con Versión ==
Version Extracted: 2.0
Deserialized Object (With Version):
ID: 1
Name: John Doe

== Validación de Datos Serializados ==
Is Serialized Data Valid? True

== Serialización de Lista ==
Serialized List:
[
  {
    "Id": 2,
    "Name": "Jane Doe",
    "BirthDate": "1995-07-15T00:00:00",
    "IsActive": false
  },
  {
    "Id": 3,
    "Name": "Jack Smith",
    "BirthDate": "1988-03-10T00:00:00",
    "IsActive": true
  }
]
```

### SystemText versioned serialize and deserialize
Methods : Serialize(), Deserialize(), SerializeCompressed(), DeserializeCompressed(), SerializeWithVersion(), DeserializeWithVersion(), ValidateSerializedData(), SerializeMessages()
````csharp
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Implementations.Versioned;

namespace ExampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var person = new Person
            {
                Id = 1,
                Name = "John Doe",
                BirthDate = new DateTime(1990, 5, 1),
                IsActive = true,
                Version = "1.0"
            };

            var serializer = new SystemTextSerializerWithVersion<Person>();

            Console.WriteLine("== Serialización ==");
            var serializedData = serializer.Serialize(person);
            Console.WriteLine($"Serialized Data:\n{serializedData}");

            Console.WriteLine("\n== Deserialización Simple ==");
            var deserializedPerson = serializer.Deserialize(serializedData);
            Console.WriteLine($"Deserialized Person:\nID: {deserializedPerson.Id}\nName: {deserializedPerson.Name}");

            Console.WriteLine("\n== Serialización Comprimida ==");
            var compressedData = serializer.SerializeCompressed(person);
            Console.WriteLine($"Compressed Data (byte array length): {compressedData.Length}");

            Console.WriteLine("\n== Deserialización Comprimida ==");
            var decompressedPerson = serializer.DeserializeCompressed(compressedData);
            Console.WriteLine($"Decompressed Person:\nID: {decompressedPerson.Id}\nName: {decompressedPerson.Name}");

            Console.WriteLine("\n== Serialización con Versión ==");
            var serializedWithVersion = serializer.SerializeWithVersion(person, "2.0");
            Console.WriteLine($"Serialized Data with Version:\n{serializedWithVersion}");

            Console.WriteLine("\n== Deserialización con Versión ==");
            var (deserializedWithVersion, version) = serializer.DeserializeWithVersion(serializedWithVersion);
            Console.WriteLine($"Version Extracted: {version}");
            Console.WriteLine($"Deserialized Object (With Version):\nID: {deserializedWithVersion.Id}\nName: {deserializedWithVersion.Name}");

            Console.WriteLine("\n== Validación de Datos Serializados ==");
            var isValid = serializer.ValidateSerializedData(serializedData, null);
            Console.WriteLine($"Is Serialized Data Valid? {isValid}");

            Console.WriteLine("\n== Serialización de Lista ==");
            var people = new List<Person>
            {
                new Person { Id = 2, Name = "Jane Doe", BirthDate = new DateTime(1995, 7, 15), IsActive = false },
                new Person { Id = 3, Name = "Jack Smith", BirthDate = new DateTime(1988, 3, 10), IsActive = true }
            };
            var serializedMessages = serializer.SerializeMessages(people);
            Console.WriteLine($"Serialized List:\n{serializedMessages}");
        }
    }

    public class Person : IVersioned
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
        public string Version { get; set; }
    }
}
````
Output
```
== Serialización ==
Serialized Data:
{
  "id": 1,
  "name": "John Doe",
  "birthDate": "1990-05-01T00:00:00",
  "isActive": true,
  "version": "1.0"
}

== Deserialización Simple ==
Deserialized Person:
ID: 1
Name: John Doe

== Serialización Comprimida ==
Compressed Data (byte array length): 109

== Deserialización Comprimida ==
Decompressed Person:
ID: 1
== Serialización con Versión ==
Serialized Data with Version:
{
  "id": 1,
  "name": "John Doe",
  "birthDate": "1990-05-01T00:00:00",
  "isActive": true,
  "version": "2.0"
}

== Deserialización con Versión ==
Version Extracted: 2.0
Deserialized Object (With Version):
ID: 1
Name: John Doe

== Validación de Datos Serializados ==
Is Serialized Data Valid? True

== Serialización de Lista ==
Serialized List:
[
  {
    "id": 2,
    "name": "Jane Doe",
    "birthDate": "1995-07-15T00:00:00",
    "isActive": false,
    "version": null
  },
  {
    "id": 3,
    "name": "Jack Smith",
    "birthDate": "1988-03-10T00:00:00",
    "isActive": true,
    "version": null
  }
]
```

### Xml versioned serialize and deserialize
Methods : Serialize(), Deserialize(), SerializeCompressed(), DeserializeCompressed(), SerializeWithVersion(), DeserializeWithVersion(), ValidateSerializedData(), SerializeMessages()
````csharp
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Implementations.Versioned;

namespace ExampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var person = new Person
            {
                Id = 1,
                Name = "John Doe",
                BirthDate = new DateTime(1990, 5, 1),
                IsActive = true,
                Version = "1.0"
            };

            var serializer = new XmlSerializerWithVersion<Person>();

            Console.WriteLine("== Serialización ==");
            var serializedData = serializer.Serialize(person);
            Console.WriteLine($"Serialized Data:\n{serializedData}");

            Console.WriteLine("\n== Deserialización Simple ==");
            var deserializedPerson = serializer.Deserialize(serializedData);
            Console.WriteLine($"Deserialized Person:\nID: {deserializedPerson.Id}\nName: {deserializedPerson.Name}");

            Console.WriteLine("\n== Serialización Comprimida ==");
            var compressedData = serializer.SerializeCompressed(person);
            Console.WriteLine($"Compressed Data (byte array length): {compressedData.Length}");

            Console.WriteLine("\n== Deserialización Comprimida ==");
            var decompressedPerson = serializer.DeserializeCompressed(compressedData);
            Console.WriteLine($"Decompressed Person:\nID: {decompressedPerson.Id}\nName: {decompressedPerson.Name}");

            Console.WriteLine("\n== Serialización con Versión ==");
            var serializedWithVersion = serializer.SerializeWithVersion(person, "2.0");
            Console.WriteLine($"Serialized Data with Version:\n{serializedWithVersion}");

            Console.WriteLine("\n== Deserialización con Versión ==");
            var (deserializedWithVersion, version) = serializer.DeserializeWithVersion(serializedWithVersion);
            Console.WriteLine($"Version Extracted: {version}");
            Console.WriteLine($"Deserialized Object (With Version):\nID: {deserializedWithVersion.Id}\nName: {deserializedWithVersion.Name}");

            Console.WriteLine("\n== Validación de Datos Serializados ==");
            var isValid = serializer.ValidateSerializedData(serializedData, null);
            Console.WriteLine($"Is Serialized Data Valid? {isValid}");

            Console.WriteLine("\n== Serialización de Lista ==");
            var people = new List<Person>
            {
                new Person { Id = 2, Name = "Jane Doe", BirthDate = new DateTime(1995, 7, 15), IsActive = false },
                new Person { Id = 3, Name = "Jack Smith", BirthDate = new DateTime(1988, 3, 10), IsActive = true }
            };
            var serializedMessages = serializer.SerializeMessages(people);
            Console.WriteLine($"Serialized List:\n{serializedMessages}");
        }
    }

    public class Person : IVersioned
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
        public string Version { get; set; }
    }
}
````
Ouput
```
== Serialización ==
Serialized Data:
<?xml version="1.0" encoding="utf-16"?>
<Person xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Id>1</Id>
  <Name>John Doe</Name>
  <BirthDate>1990-05-01T00:00:00</BirthDate>
  <IsActive>true</IsActive>
  <Version>1.0</Version>
</Person>


== Deserialización Simple ==
Deserialized Person:
ID: 1
Name: John Doe

== Serialización Comprimida ==
Compressed Data (byte array length): 210
== Deserialización Comprimida ==
Decompressed Person:
ID: 1
Name: John Doe

== Serialización con Versión ==
Serialized Data with Version:
<?xml version="1.0" encoding="utf-16"?>
<Person xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://
                                                                               /www.w3.org/2001/XMLSchema">
  <Id>1</Id>
  <Name>John Doe</Name>
  <BirthDate>1990-05-01T00:00:00</BirthDate>
  <IsActive>true</IsActive>
  <Version>2.0</Version>
</Person>

== Deserialización con Versión ==
Version Extracted: 2.0
Deserialized Object (With Version):
ID: 1
Name: John Doe

== Validación de Datos Serializados ==
Is Serialized Data Valid? True

== Serialización de Lista ==
Serialized List:
<?xml version="1.0" encoding="utf-16"?>
<ArrayOfPerson xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="
                                                                               "http://www.w3.org/2001/XMLSchema">
  <Person>
    <Id>2</Id>
    <Name>Jane Doe</Name>
    <BirthDate>1995-07-15T00:00:00</BirthDate>
    <IsActive>false</IsActive>
  </Person>
  <Person>
    <Id>3</Id>
    <Name>Jack Smith</Name>
    <BirthDate>1988-03-10T00:00:00</BirthDate>
    <IsActive>true</IsActive>
  </Person>
</ArrayOfPerson>
```

### Yaml versioned serialize and deserialize
Methods: Serialize(), Deserialize(), SerializeCompressed(), SerializeCompressed(), SerializeWithVersion(), DeserializeWithVersion(), ValidateSerializedData(), SerializeMessages()
````csharp
using NetNinja.Serializers.Abstractions;
using NetNinja.Serializers.Implementations.Versioned;

namespace ExampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var person = new Person
            {
                Id = 1,
                Name = "John Doe",
                BirthDate = new DateTime(1990, 5, 1),
                IsActive = true,
                Version = "1.0"
            };

            var serializer = new YamlSerializerWithVersion<Person>();

            Console.WriteLine("== Serialización ==");
            var serializedData = serializer.Serialize(person);
            Console.WriteLine($"Serialized Data:\n{serializedData}");

            Console.WriteLine("\n== Deserialización Simple ==");
            var deserializedPerson = serializer.Deserialize(serializedData);
            Console.WriteLine($"Deserialized Person:\nID: {deserializedPerson.Id}\nName: {deserializedPerson.Name}");

            Console.WriteLine("\n== Serialización Comprimida ==");
            var compressedData = serializer.SerializeCompressed(person);
            Console.WriteLine($"Compressed Data (byte array length): {compressedData.Length}");

            Console.WriteLine("\n== Deserialización Comprimida ==");
            var decompressedPerson = serializer.DeserializeCompressed(compressedData);
            Console.WriteLine($"Decompressed Person:\nID: {decompressedPerson.Id}\nName: {decompressedPerson.Name}");

            Console.WriteLine("\n== Serialización con Versión ==");
            var serializedWithVersion = serializer.SerializeWithVersion(person, "2.0");
            Console.WriteLine($"Serialized Data with Version:\n{serializedWithVersion}");

            Console.WriteLine("\n== Deserialización con Versión ==");
            var (deserializedWithVersion, version) = serializer.DeserializeWithVersion(serializedWithVersion);
            Console.WriteLine($"Version Extracted: {version}");
            Console.WriteLine($"Deserialized Object (With Version):\nID: {deserializedWithVersion.Id}\nName: {deserializedWithVersion.Name}");

            Console.WriteLine("\n== Validación de Datos Serializados ==");
            var isValid = serializer.ValidateSerializedData(serializedData, null);
            Console.WriteLine($"Is Serialized Data Valid? {isValid}");

            Console.WriteLine("\n== Serialización de Lista ==");
            var people = new List<Person>
            {
                new Person { Id = 2, Name = "Jane Doe", BirthDate = new DateTime(1995, 7, 15), IsActive = false },
                new Person { Id = 3, Name = "Jack Smith", BirthDate = new DateTime(1988, 3, 10), IsActive = true }
            };
            var serializedMessages = serializer.SerializeMessages(people);
            Console.WriteLine($"Serialized List:\n{serializedMessages}");
        }
    }

    public class Person : IVersioned
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
        public string Version { get; set; }
    }
}
````
Output
```
== Serialización ==
Serialized Data:
id: 1
name: John Doe
birthDate: 1990-05-01T00:00:00.0000000
isActive: true
version: 1.0


== Deserialización Simple ==
Deserialized Person:
ID: 1
Name: John Doe

== Serialización Comprimida ==
Compressed Data (byte array length): 99

== Deserialización Comprimida ==
Decompressed Person:
ID: 1
== Serialización con Versión ==
Serialized Data with Version:
id: 1
name: John Doe
birthDate: 1990-05-01T00:00:00.0000000
isActive: true
version: 2.0


== Deserialización con Versión ==
Version Extracted: 2.0
Deserialized Object (With Version):
ID: 1
Name: John Doe

== Validación de Datos Serializados ==
Is Serialized Data Valid? True

== Serialización de Lista ==
Serialized List:
- id: 2
  name: Jane Doe
  birthDate: 1995-07-15T00:00:00.0000000
  isActive: false
  version:
- id: 3
  name: Jack Smith
  birthDate: 1988-03-10T00:00:00.0000000
  isActive: true
  version:
```

### Serialize and Deserialize with destination
````csharp
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NetNinja.Serializers.Implementations.ForHooks;
using NetNinja.Serializers.Implementations.WithDestinations;
using NetNinja.Serializers.Helpers;
using NetNinja.Serializers.Configurations;

namespace ExampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            
            var logger = loggerFactory.CreateLogger<SerializerWithDestinations<Person>>();
            
            var hooksLogger = loggerFactory.CreateLogger<JsonSerializerWithHooks<Person>>();
            
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var jsonSerializerWithHooks = new JsonSerializerWithHooks<Person>(
                null,
                hooksLogger,
                jsonSettings,
                enableEncryption: false
            );

            var serializer = new SerializerWithDestinations<Person>(jsonSerializerWithHooks, logger);

            var filePath = "your_absolute_path\\person_data.json";

            var person = new Person
            {
                Id = 1,
                Name = "Alice Johnson",
                BirthDate = new DateTime(1990, 1, 1),
                IsActive = true
            };

            try
            {
                logger.LogInformation("\n== Serializando el objeto a un archivo ==");
                serializer.SerializeToFile(person, filePath, format: "Indented");

                logger.LogInformation("\n== Deserializando el archivo a un objeto ==");
                var deserializedPerson = serializer.DeserializeFromFile(filePath);

                logger.LogInformation("\n== Objeto deserializado ==");
                logger.LogInformation($"ID: {deserializedPerson.Id}");
                logger.LogInformation($"Name: {deserializedPerson.Name}");
                logger.LogInformation($"BirthDate: {deserializedPerson.BirthDate}");
                logger.LogInformation($"IsActive: {deserializedPerson.IsActive}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    logger.LogInformation($"\nEl archivo '{filePath}' fue eliminado.");
                }
            }
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
    }
}
````
Output
```
If you enable encription : 

var encryptionHelper = new EncryptionHelper(
    new EncryptionConfiguration
    {
        EncryptionKey = "234234234-234234"
    });

var jsonSettings = new JsonSerializerSettings
{
    Formatting = Formatting.Indented
};

var jsonSerializerWithHooks = new JsonSerializerWithHooks<Person>(
    encryptionHelper,
    hooksLogger,
    jsonSettings,
    enableEncryption: true
);

in the person_data.json you will have an encrypted data ,

If you disable and comment deletions lines you should see something like that
{
  "Id": 1,
  "Name": "Alice Johnson",
  "BirthDate": "1990-01-01T00:00:00",
  "IsActive": true
}
```

### Send the data to serialize and deserialize encripted
You can send the data encrypted in all the methods of the class JsonSerializerWithHooks like this
````csharp
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NetNinja.Serializers.Implementations.ForHooks;
using NetNinja.Serializers.Implementations.WithDestinations;
using NetNinja.Serializers.Helpers;
using NetNinja.Serializers.Configurations;

namespace ExampleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            
            var logger = loggerFactory.CreateLogger<SerializerWithDestinations<Person>>();
            
            var hooksLogger = loggerFactory.CreateLogger<JsonSerializerWithHooks<Person>>();
            
            var encryptionHelper = new EncryptionHelper(
                new EncryptionConfiguration
                {
                    EncryptionKey = "234234234-234234"
                });

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var jsonSerializerWithHooks = new JsonSerializerWithHooks<Person>(
                encryptionHelper,
                hooksLogger,
                jsonSettings,
                enableEncryption: false
            );

            var serializer = new SerializerWithDestinations<Person>(jsonSerializerWithHooks, logger);
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
    }
}
````

## Installation

To install `NetNinja.Serializers`, run the following command:

```bash
dotnet add package NetNinja.Serializers --version 0.0.4
```

Or use the NuGet package manager UI in Visual Studio.
---

## License

This project is licensed under the [MIT License](./LICENSE).

---

## Contributing

If you encounter any issues or have ideas for enhancements, feel free to open an issue or submit a pull request directly on our [GitHub repository](https://github.com/christian-cell/NetNinja.Serializers)!

---