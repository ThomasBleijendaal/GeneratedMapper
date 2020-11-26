using GeneratedMapper.Abstractions;
using GeneratedMapper.Builders;
using GeneratedMapper.Configurations;
using GeneratedMapper.Information;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.Tests
{
    internal class MappingBuilderTests
    {
        private static readonly ConfigurationValues Values = new ConfigurationValues(IndentStyle.Space, 4);

        private static readonly Mock<ITypeSymbol> SourceType = new Mock<ITypeSymbol>();
        private static readonly Mock<ITypeSymbol> DestinationType = new Mock<ITypeSymbol>();

        private static readonly Mock<ITypeSymbol> NestedSourceType = new Mock<ITypeSymbol>();
        private static readonly Mock<ITypeSymbol> NestedDestinationType = new Mock<ITypeSymbol>();

        [SetUp]
        public void Setup()
        {
            var @namespace = new Mock<INamespaceSymbol>();
            @namespace.Setup(x => x.Name).Returns("Namespace");
            @namespace.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace");

            SourceType.Setup(x => x.Name).Returns("Source");
            SourceType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            DestinationType.Setup(x => x.Name).Returns("Destination");
            DestinationType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);

            NestedSourceType.Setup(x => x.Name).Returns("SourceObject");
            NestedSourceType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            NestedDestinationType.Setup(x => x.Name).Returns("DestinationObject");
            NestedDestinationType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestCodeGeneration(PropertyMappingInformation[] mappings, string expectedSourceText)
        {
            var information = new MappingInformation(DestinationType.Object, Enumerable.Empty<Diagnostic>(), mappings, SourceType.Object);
            var builder = new MappingBuilder(information, Values);

            Assert.AreEqual(expectedSourceText, builder.GenerateSourceText().ToString());
        }

        private static IEnumerable<TestCaseData> TestCases()
        { 
            yield return MappingSinglePropertyFromSourceToDestination()
                .SetName(nameof(MappingSinglePropertyFromSourceToDestination));
            
            yield return MappingThreePropertiesFromSourceToDestination()
                .SetName(nameof(MappingThreePropertiesFromSourceToDestination));
            
            yield return MappingSinglePropertyWithDifferentNamesFromSourceToDestination()
                .SetName(nameof(MappingSinglePropertyWithDifferentNamesFromSourceToDestination));

            yield return MappingPropertyToStringFromSourceToDestination()
                .SetName(nameof(MappingPropertyToStringFromSourceToDestination));

            yield return MappingNullablePropertyToStringFromSourceToDestination()
                .SetName(nameof(MappingNullablePropertyToStringFromSourceToDestination));

            yield return MappingPropertyUsingResolverFromSourceToDestination()
                .SetName(nameof(MappingPropertyUsingResolverFromSourceToDestination));
            
            yield return MappingPropertyUsingResolverWithConstructorArgumentsFromSourceToDestination()
                .SetName(nameof(MappingPropertyUsingResolverWithConstructorArgumentsFromSourceToDestination));
            
            yield return MappingMultiplePropertiesUsingResolversWithConstructorArgumentsFromSourceToDestination()
                .SetName(nameof(MappingMultiplePropertiesUsingResolversWithConstructorArgumentsFromSourceToDestination));
            
            yield return MappingMultiplePropertiesUsingSameResolversWithConstructorArgumentsFromSourceToDestination()
                .SetName(nameof(MappingMultiplePropertiesUsingSameResolversWithConstructorArgumentsFromSourceToDestination));

            yield return MappingSingleEnumerablePropertyWithMethodToListFromSourceToDestination()
                .SetName(nameof(MappingSingleEnumerablePropertyWithMethodToListFromSourceToDestination));

            yield return MappingSingleEnumerablePropertyToListFromSourceToDestination()
                .SetName(nameof(MappingSingleEnumerablePropertyToListFromSourceToDestination));

            yield return MappingSingleNullableEnumerablePropertyToListFromSourceToDestination()
                .SetName(nameof(MappingSingleNullableEnumerablePropertyToListFromSourceToDestination));

            yield return MappingSingleNullableEnumerablePropertyToArrayFromSourceToDestination()
                .SetName(nameof(MappingSingleNullableEnumerablePropertyToArrayFromSourceToDestination));

            yield return MappingSingleNullableEnumerablePropertyToArrayUsingResolverFromSourceToDestination()
                .SetName(nameof(MappingSingleNullableEnumerablePropertyToArrayUsingResolverFromSourceToDestination));

            yield return MappingPropertyUsingMapperOfDestinationType()
                .SetName(nameof(MappingPropertyUsingMapperOfDestinationType));

            yield return MappingPropertyUsingMapperOfDestinationTypeWithArguments()
                .SetName(nameof(MappingPropertyUsingMapperOfDestinationTypeWithArguments));

            yield return MappingCollectionPropertyUsingMapperOfDestinationTypeWithArguments()
                .SetName(nameof(MappingCollectionPropertyUsingMapperOfDestinationTypeWithArguments));
        }

        private static TestCaseData MappingSinglePropertyFromSourceToDestination()
            => new TestCaseData(
                new[] 
                { 
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("Name", false).MapTo("Name", false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingThreePropertiesFromSourceToDestination()
            => new TestCaseData(
                new[] 
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("Name", false).MapTo("Name", false),
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("Title", false).MapTo("Title", false),
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("Content", false).MapTo("Content", false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
                Title = self.Title,
                Content = self.Content,
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingSinglePropertyWithDifferentNamesFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("Name", false).MapTo("Nom", false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Nom = self.Name,
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingPropertyToStringFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("Count", false).MapTo("Count", false).UsingMethod("ToString", default)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Count = self.Count.ToString(),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingNullablePropertyToStringFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("Count", true).MapTo("Count", false).UsingMethod("ToString", default)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Count = self.Count?.ToString(),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingPropertyUsingResolverFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("Parameter", false).MapTo("ResolvedParameter", false).UsingResolver("Resolver", "Namespace.Resolvers", Enumerable.Empty<MethodParameter>())
                },
                @"using System;
using Namespace;
using Namespace.Resolvers;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var resolver = new Resolver();
            
            var target = new Destination
            {
                ResolvedParameter = resolver.Resolve(self.Parameter),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingPropertyUsingResolverWithConstructorArgumentsFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Parameter", false)
                        .MapTo("ResolvedParameter", false)
                        .UsingResolver("SomeResolver", "Namespace.Resolvers", new [] {
                            new MethodParameter("randomService", "IRandomService", "Namespace.Services", default)
                        })
                },
                @"using System;
using Namespace;
using Namespace.Resolvers;
using Namespace.Services;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, IRandomService someResolverRandomService)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var someResolver = new SomeResolver(someResolverRandomService);
            
            var target = new Destination
            {
                ResolvedParameter = someResolver.Resolve(self.Parameter),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingMultiplePropertiesUsingResolversWithConstructorArgumentsFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Parameter1", false)
                        .MapTo("ResolvedParameter1", false)
                        .UsingResolver("SomeResolver1", "Namespace.Resolvers1", new [] {
                            new MethodParameter("randomService1", "IRandomService1", "Namespace.Services1", default)
                        }),
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Parameter2", false)
                        .MapTo("ResolvedParameter2", false)
                        .UsingResolver("SomeResolver2", "Namespace.Resolvers2", new [] {
                            new MethodParameter("randomService2", "IRandomService2", "Namespace.Services2", default)
                        })
                },
                @"using System;
using Namespace;
using Namespace.Resolvers1;
using Namespace.Resolvers2;
using Namespace.Services1;
using Namespace.Services2;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, IRandomService1 someResolver1RandomService1, IRandomService2 someResolver2RandomService2)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var someResolver1 = new SomeResolver1(someResolver1RandomService1);
            
            var someResolver2 = new SomeResolver2(someResolver2RandomService2);
            
            var target = new Destination
            {
                ResolvedParameter1 = someResolver1.Resolve(self.Parameter1),
                ResolvedParameter2 = someResolver2.Resolve(self.Parameter2),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingMultiplePropertiesUsingSameResolversWithConstructorArgumentsFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Parameter1", false)
                        .MapTo("ResolvedParameter1", false)
                        .UsingResolver("SomeResolver", "Namespace.Resolvers", new [] {
                            new MethodParameter("randomService", "IRandomService", "Namespace.Services", default)
                        }),
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Parameter2", false)
                        .MapTo("ResolvedParameter2", false)
                        .UsingResolver("SomeResolver", "Namespace.Resolvers", new [] {
                            new MethodParameter("randomService", "IRandomService", "Namespace.Services", default)
                        })
                },
                @"using System;
using Namespace;
using Namespace.Resolvers;
using Namespace.Services;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, IRandomService someResolverRandomService)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var someResolver = new SomeResolver(someResolverRandomService);
            
            var target = new Destination
            {
                ResolvedParameter1 = someResolver.Resolve(self.Parameter1),
                ResolvedParameter2 = someResolver.Resolve(self.Parameter2),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingSingleEnumerablePropertyWithMethodToListFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Collection", false)
                        .MapTo("Collection", false)
                        .AsCollection(DestinationCollectionType.List, "CollectionItem", "Namespace.Collections")
                        .UsingMethod("ToItem", "Namespace.Collections")
                },
                @"using System;
using System.Linq;
using Namespace;
using Namespace.Collections;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Collection = self.Collection.Select(element => element.ToItem()).ToList(),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingSingleNullableEnumerablePropertyToListFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Collection", true)
                        .MapTo("Collection", false)
                        .AsCollection(DestinationCollectionType.Array, "CollectionItem", "Namespace.Collections")
                },
                @"using System;
using System.Linq;
using Namespace;
using Namespace.Collections;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Collection = self.Collection?.ToArray() ?? Enumerable.Empty<CollectionItem>().ToArray(),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingSingleEnumerablePropertyToListFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Collection", false)
                        .MapTo("Collection", false)
                        .AsCollection(DestinationCollectionType.Array, "CollectionItem", "Namespace.Collections")
                },
                @"using System;
using System.Linq;
using Namespace;
using Namespace.Collections;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Collection = self.Collection.ToArray(),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingSingleNullableEnumerablePropertyToArrayFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Collection", true)
                        .MapTo("Collection", false)
                        .AsCollection(DestinationCollectionType.Array, "CollectionItem", "Namespace.Collections")
                        .UsingMethod("MapToCollectionItem", "Namespace.Collections")
                },
                @"using System;
using System.Linq;
using Namespace;
using Namespace.Collections;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Collection = self.Collection?.Select(element => element.MapToCollectionItem()).ToArray() ?? Enumerable.Empty<CollectionItem>().ToArray(),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingSingleNullableEnumerablePropertyToArrayUsingResolverFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("Collection", true)
                        .MapTo("Collection", false)
                        .AsCollection(DestinationCollectionType.Array, "CollectionItem", "Namespace.Collections")
                        .UsingResolver("CollectionResolver", "Namespace.Resolvers", new [] {
                            new MethodParameter("cultureInfo", "CultureInfo", "System.Globalization", "CultureInfo.InvariantCulture")
                        })
                },
                @"using System;
using System.Globalization;
using System.Linq;
using Namespace;
using Namespace.Collections;
using Namespace.Resolvers;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, CultureInfo collectionResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var collectionResolver = new CollectionResolver(collectionResolverCultureInfo);
            
            var target = new Destination
            {
                Collection = self.Collection?.Select(element => collectionResolver.Resolve(element)).ToArray() ?? Enumerable.Empty<CollectionItem>().ToArray(),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingPropertyUsingMapperOfDestinationType()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("NestedObject", true)
                        .MapTo("NestedObject", false)
                        .UsingMapper(NestedSourceType.Object, NestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(NestedDestinationType.Object, Enumerable.Empty<Diagnostic>(), new [] {
                            new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                                .MapFrom("Name", false)
                                .MapTo("Name", false)
                        }, NestedSourceType.Object))
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                NestedObject = self.NestedObject?.MapToDestinationObject(),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingPropertyUsingMapperOfDestinationTypeWithArguments()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("NestedObject", true)
                        .MapTo("NestedObject", false)
                        .UsingMapper(NestedSourceType.Object, NestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(NestedDestinationType.Object, Enumerable.Empty<Diagnostic>(), new [] {
                            new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                                .MapFrom("Name", false)
                                .MapTo("Name", false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers", new[]
                                {
                                    new MethodParameter("cultureInfo", "CultureInfo", "System.Globalization", "CultureInfo.InvariantCulture")
                                })
                        }, NestedSourceType.Object))
                },
                @"using System;
using System.Globalization;
using Namespace;
using Namespace.Resolvers;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, CultureInfo destinationObjectSomeResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                NestedObject = self.NestedObject?.MapToDestinationObject(destinationObjectSomeResolverCultureInfo),
            };
            
            return target;
        }
    }
}
");

        private static TestCaseData MappingCollectionPropertyUsingMapperOfDestinationTypeWithArguments()
            => new TestCaseData(
                new[]
                {
                    new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                        .MapFrom("NestedObject", true)
                        .MapTo("NestedObject", false)
                        .AsCollection(DestinationCollectionType.Enumerable, "DestinationObject", "Namespace")
                        .UsingMapper(NestedSourceType.Object, NestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(NestedDestinationType.Object, Enumerable.Empty<Diagnostic>(), new [] {
                            new PropertyMappingInformation(SourceType.Object, DestinationType.Object)
                                .MapFrom("Name", false)
                                .MapTo("Name", false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers", new[]
                                {
                                    new MethodParameter("cultureInfo", "CultureInfo", "System.Globalization", "CultureInfo.InvariantCulture")
                                })
                        }, NestedSourceType.Object))
                },
                @"using System;
using System.Globalization;
using System.Linq;
using Namespace;
using Namespace.Resolvers;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, CultureInfo destinationObjectSomeResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                NestedObject = self.NestedObject?.Select(element => element.MapToDestinationObject(destinationObjectSomeResolverCultureInfo)) ?? Enumerable.Empty<DestinationObject>(),
            };
            
            return target;
        }
    }
}
");

        //        private static TestCaseData MappingNextedClassPropertyFromSourceToDestination()
        //            => new TestCaseData(
        //                new[]
        //                {
        //                    new PropertyToPropertyWithMethodInvocationMapping("NestedProperty", "NestedProperty", "ToNestedDestination")
        //                },
        //                @"using System;

        //namespace Namespace
        //{
        //    public static partial class SourceMapToExtensions
        //    {
        //        public static Destination MapToDestination(this Source self)
        //        {
        //            if (self is null)
        //            {
        //                throw new ArgumentNullException(nameof(self));
        //            }

        //            var target = new Destination
        //            {
        //                NestedProperty = self.NestedProperty.ToNestedDestination(),
        //            };

        //            return target;
        //        }
        //    }
        //}
        //");
    }
}
