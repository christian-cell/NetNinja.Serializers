# Resumen de los puntos tratados

## 1. Crear método para comparar diferencias
Implementamos `GetDifferences`, un método que identifica discrepancias entre las propiedades de dos objetos deserializados.

## 2. Manejo de propiedades nulas
Se añadieron validaciones para manejar casos donde uno o ambos objetos sean `null`.

## 3. Comparación profunda de objetos
Utilizamos reflexión para recorrer y comparar todas las propiedades públicas de los objetos.

## 4. Ejemplo práctico
Probamos la funcionalidad comparando datos en JSON y XML para simular escenarios reales.

## 5. Problema inicial
Detectamos diferencias incorrectas, como: Esto reveló problemas en el proceso de deserialización.

## 6. Validación de `VersionObject`
Confirmamos que la clase `VersionObject` tenía las propiedades adecuadas: `Version` y `Name`.

## 7. Problemas encontrados en la deserialización
Identificamos dos posibles causas:
- Desajuste entre los nombres de las propiedades y los campos en los datos serializados.
- Configuración incompleta o incorrecta de los serializadores (para JSON y XML).

## 8. Configuración del serializador JSON
Para resolver problemas de deserialización, configuramos opciones como:
```csharp
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true // Ignorar diferencias en mayúsculas/minúsculas.
};
```

## 9. Configuración del serializador XML
Aseguramos que las etiquetas del XML coincidan con las propiedades utilizando atributos como:
```csharp
[XmlRoot("VersionObject")]
[XmlElement("Version")]
```

## 10. Centralización de configuraciones en JSON
Agregamos configuraciones globales directamente dentro del serializador JSON personalizado (`SystemTextSerializerWithVersion`).

## 11. Manejo de deserialización XML
Confirmamos que las etiquetas del XML se alineen correctamente con las propiedades de los objetos.

## 12. Prueba con datos equivalentes
Probamos con datos que deberían ser equivalentes:
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

El resultado esperado fue **sin diferencias**.

## 13. Resolviendo diferencias erróneas
Detectamos que algunas propiedades estaban vacías (`''`) debido a configuraciones incompletas de los serializadores. Esto fue corregido.

## 14. Implementación limpia
Centralizamos la configuración en los serializadores (como `SystemTextSerializerWithVersion`) y en la fábrica de serializadores (`SerializerFactory`).

## 15. Opciones para configuración global
Definimos 3 enfoques para aplicar configuraciones de serialización:
1. **Recomendado**: Dentro de las clases de serialización personalizadas.
2. En la fábrica de serializadores (`SerializerFactory`).
3. Localmente en casos puntuales, pero no recomendado.

## 16. Validación exitosa
Tras aplicar las correcciones, el sistema detecta correctamente que dos objetos equivalentes no presentan diferencias. Ejemplo:
```plaintext
No hay diferencias.
```

---

¡Este resumen está listo para ser usado en tu documentación! 🎉