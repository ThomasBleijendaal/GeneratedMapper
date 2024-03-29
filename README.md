# GeneratedMapper

[![#](https://img.shields.io/nuget/v/GeneratedMapper?style=flat-square)](https://www.nuget.org/packages/GeneratedMapper)

Compile-time object-to-object mapper generator which generates extension methods for each of the mappings.

```
Install-Package GeneratedMapper
```

(Sometimes VS2019 needs to be restarted after adding the package)

## Bad informercial

How often do you have a certain model like..

```c#
public class SourceDTO
{
    public string Name { get; set; }

    public string SomeProp { get; set; }
}
```

..which must be mapped to a model like..

```c#
public class DestinationViewModel
{
    public string Name { get; set; }

    public string SomeProp { get; set; }
}
```

..but you don't want the runtime surpises and performance hits of an `AutoMapper`, `ExpressionMapper`, or any other mapper?

Introducing GeneratedMapper, the compile-time object-to-object mapper that generates mappings at compile-time! Just slap on some attributes, and have your computer work for YOU!

This extension method..

```c#
public static partial class SourceDTOMapToExtensions
{
    public static DestinationViewModel MapToDestination(this SourceDTO self)
    {
        if (self is null)
        {
            throw new ArgumentNullException(nameof(self));
        }
            
        var target = new DestinationViewModel
        {
            Name = self.Name,
            SomeProp = self.SomeProp
        };
            
        return target;
    }
}
```

..can be yours if pick up your phone now and add the following attribute to your class:

```c#
[MapTo(typeof(DestinationViewModel))]
public class SourceDTO
{
    public string Name { get; set; }

    public string SomeProp { get; set; }
}
```

Don't delay, generate your object-to-object mappers at compile-time today!

## Features

### Mapping

- [Property to property mapping (`Title` -> `target.Title = source.Title`).](https://github.com/ThomasBleijendaal/GeneratedMapper/blob/main/src/GeneratedMapper.Tests/BasicMappingGeneratorTests.cs)
- Property to property mapping with different names (`[MapWith("TheTitle")] Title` -> `target.TheTitle = source.Title`).
- [Property to property mapping using (extension) method (`[MapWith("TheTitle", "Substring")] Title` -> `target.TheTitle = source.Title.Substring(startIndex)`).](https://github.com/ThomasBleijendaal/GeneratedMapper/blob/main/src/GeneratedMapper.Tests/BasicExtensionMethodMappingGeneratorTests.cs)
- [Property to property mapping using resolver (`[MapWith("TheTitle", typeof(Resolver))] Title` -> `target.TheTitle = resolver.Resolve(source.Title)`).](https://github.com/ThomasBleijendaal/GeneratedMapper/blob/main/src/GeneratedMapper.Tests/BasicResolverMappingGeneratorTests.cs)
- [Enumeration mapping (`Codes` -> `target.Codes = (source.Codes ?? Enumerable.Empty<string>()).ToArray()`).](https://github.com/ThomasBleijendaal/GeneratedMapper/blob/main/src/GeneratedMapper.Tests/CollectionEnumerableMappingGeneratorTests.cs)
- [Dictionary mapping (`Definitions` -> `target.Definitions = (source.Codes ?? Enumerable.Empty<KeyValuePair<string, string>>()).ToDictionary(x => x.Key, x => x.Value)`).](https://github.com/ThomasBleijendaal/GeneratedMapper/blob/main/src/GeneratedMapper.Tests/DictionaryMapperGeneratorTests.cs)
- [Async mapping using `[MapAsyncWith("Property", "AsyncMethodAsync")]`, `[MapAsyncWith("Property", "AsyncExtensionMethodAsync")]`, or `[MapAsyncWith("Property", typeof(AsyncResolver))]`.](https://github.com/ThomasBleijendaal/GeneratedMapper/blob/main/src/GeneratedMapper.Tests/AsyncMappingGeneratorTests.cs)
- [Simple nested property mapping (`[MapWith("Sys.Id")]Id` -> `target.Id = source.Sys?.Id`)](https://github.com/ThomasBleijendaal/GeneratedMapper/blob/main/src/GeneratedMapper.Tests/NestedMappingGeneratorTests.cs)

### Configuration

Use `[assembly: MapperGeneratorConfiguration()]` to configure the mapper, like always including certain namespaces if the mapper fails to recognize them,
or configure what exceptions should be thrown when the mapper encounters null.

- `ThrowWhenNotNullablePropertyIsNull`: Configures whether the mapper should throw when a non-nullable property is null.
- `ThrowWhenNotNullableElementIsNull`: Configures whether the mapper should throw when a non-nullable element in a collection is null.
- `NamespacesToInclude`: Configures extra namespaces to include in mappers when they prove to be too hard to be recognized by the mapper.
- `GenerateEnumerableMethods`: Configures if for every X.MapToY() also a IEnumerable<X>.MapToYs() must be generated.
- `GenerateExpressions`: Configures if for every mapper, the mapping should also be available as `Expression<Func<X, Y>>` for use in APIs which require expressions (like EF or Mongo).
- `GenerateInjectableMappers`: Generates a `IMapper<From, To>` for each extension method, which can be added to the DI container using `services.AddMappers()`.

## More information

Please review the Example project to find more examples. The [GeneratedMapper.Tests project](https://github.com/ThomasBleijendaal/GeneratedMapper/tree/main/src/GeneratedMapper.Tests) contains
a lot of unit tests which also show what is possible.
