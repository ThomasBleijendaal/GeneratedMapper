# GeneratedMapper
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

## More information

Please review the Example project to find more examples. The GeneratedMapper.Tests project contains
a lot of unit tests which also show what's possible.