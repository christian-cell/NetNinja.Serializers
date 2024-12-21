# Summary of Discussed Points

## 1. Create method to compare differences
Implemented `GetDifferences`, a method that identifies discrepancies between the properties of two deserialized objects.

## 2. Handling null properties
Added validations to handle cases where one or both objects are `null`.

## 3. Deep object comparison
Used reflection to traverse and compare all public properties of the objects.

## 4. Practical example
Tested the functionality by comparing data in JSON and XML to simulate real-world scenarios.

## 5. Initial issue
Detected incorrect differences, such as: This exposed issues in the deserialization process.

## 6. Validation of `VersionObject`
Confirmed that the `VersionObject` class had the appropriate properties: `Version` and `Name`.

## 7. Issues found in deserialization
Identified two possible causes:
- Mismatch between the property names and the serialized data fields.
- Incomplete or incorrect configuration of serializers (for JSON and XML).

## 8. JSON serializer configuration
To fix deserialization issues, configured options such as:
```csharp
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true // Ignore case sensitivity in property names.
};
```

## 9. XML serializer configuration
Ensured that XML tags align with the object's properties using attributes such as:
```csharp
[XmlRoot("VersionObject")]
[XmlElement("Version")]
```

## 10. JSON configuration centralization
Added global configurations directly inside the custom JSON serializer (`SystemTextSerializerWithVersion`).

## 11. XML deserialization handling
Confirmed that XML tags align properly with the properties of the deserialized objects.

## 12. Test with equivalent data
Validated the comparison with data that should be equivalent:
- **JSON**:
```json
{
  "Version": "1.0",
  "Name": "Test"
}
```

- **XML**:
```xml
<VersionObject>
  <Version>1.0</Version>
  <Name>Test</Name>
</VersionObject>
```

The expected result was **no differences**.

## 13. Fixing erroneous differences
Identified that some properties were empty (`''`) due to incomplete serializer configurations. This issue was resolved.

## 14. Clean implementation
Centralized configurations within the serializers (e.g., `SystemTextSerializerWithVersion`) and in the serializer factory (`SerializerFactory`).

## 15. Options for global configuration
Outlined three approaches to applying serialization configurations:
1. **Recommended**: Within custom serialization classes.
2. In the serializer factory (`SerializerFactory`).
3. Locally in specific cases, not recommended for production.

## 16. Successful validation
After applying the fixes, the system correctly identifies that two equivalent objects have no differences. Example:
```plaintext
No differences.
```

---

This summary is ready to be used in your documentation! 🎉