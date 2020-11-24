using GeneratedMapper.Abstractions;
using GeneratedMapper.Configurations;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.Tests
{
    public class MappingBuilderTests
    {
        private readonly ConfigurationValues _values = new ConfigurationValues(IndentStyle.Space, 4);

        private readonly Mock<ITypeSymbol> _sourceType = new Mock<ITypeSymbol>();
        private readonly Mock<ITypeSymbol> _destinationType = new Mock<ITypeSymbol>();

        [SetUp]
        public void Setup()
        {
            var @namespace = new Mock<INamespaceSymbol>();
            @namespace.Setup(x => x.Name).Returns("Namespace");
            @namespace.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace");

            _sourceType.Setup(x => x.Name).Returns("Source");
            _sourceType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            _destinationType.Setup(x => x.Name).Returns("Destination");
            _destinationType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
        }

        [TestCaseSource(nameof(TestCases))]
        public void TestCodeGeneration(IMapping[] mappings, string expectedSourceText)
        {
            var information = new MappingInformation(_destinationType.Object, Enumerable.Empty<Diagnostic>(), mappings, _sourceType.Object);
            var builder = new MappingBuilder(information, _values);

            Assert.AreEqual(expectedSourceText, builder.Text.ToString());
        }

        private static IEnumerable<TestCaseData> TestCases()
        {
            yield return MappingSinglePropertyFromSourceToDestination();
            yield return MappingThreePropertiesFromSourceToDestination();
            yield return MappingSinglePropertyWithDifferentNamesFromSourceToDestination();
            yield return MappingPropertyToStringFromSourceToDestination();
            yield return MappingPropertyUsingResolverFromSourceToDestination();
            yield return MappingPropertyUsingResolverWithConstructorArgumentsFromSourceToDestination();
            yield return MappingMultiplePropertiesUsingResolversWithConstructorArgumentsFromSourceToDestination();
            yield return MappingSingleEnumerablePropertyToListFromSourceToDestination();
        }

        private static TestCaseData MappingSinglePropertyFromSourceToDestination()
            => new TestCaseData(
                new[] 
                { 
                    new PropertyToPropertyMapping("Name", "Name")
                },
                @"using System;
using System.Linq;
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
                    new PropertyToPropertyMapping("Name", "Name"),
                    new PropertyToPropertyMapping("Title", "Title"),
                    new PropertyToPropertyMapping("Content", "Content"),
                },
                @"using System;
using System.Linq;
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
                    new PropertyToPropertyMapping("Name", "Nom")
                },
                @"using System;
using System.Linq;
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
                    new PropertyToPropertyWithMethodInvocationMapping("Count", "Count", "ToString")
                },
                @"using System;
using System.Linq;
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

        private static TestCaseData MappingPropertyUsingResolverFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyResolverMapping("Parameter", "ResolvedParameter", "Resolver", "Namespace.Resolvers", Enumerable.Empty<ConstructorParameter>())
                },
                @"using System;
using System.Linq;
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
            
            var resolverResolvedParameter = new Resolver();
            
            var target = new Destination
            {
                ResolvedParameter = resolverResolvedParameter.Resolve(self.Parameter),
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
                    new PropertyResolverMapping(
                        "Parameter", 
                        "ResolvedParameter", 
                        "SomeResolver", 
                        "Namespace.Resolvers",
                        new [] {
                            new ConstructorParameter("randomService", "IRandomService", "Namespace.Services")
                        })
                },
                @"using System;
using System.Linq;
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
            
            var resolverResolvedParameter = new SomeResolver(someResolverRandomService);
            
            var target = new Destination
            {
                ResolvedParameter = resolverResolvedParameter.Resolve(self.Parameter),
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
                    new PropertyResolverMapping(
                        "Parameter1",
                        "ResolvedParameter1",
                        "SomeResolver1",
                        "Namespace.Resolvers1",
                        new [] {
                            new ConstructorParameter("randomService1", "IRandomService1", "Namespace.Services1")
                        }),
                    new PropertyResolverMapping(
                        "Parameter2",
                        "ResolvedParameter2",
                        "SomeResolver2",
                        "Namespace.Resolvers2",
                        new [] {
                            new ConstructorParameter("randomService2", "IRandomService2", "Namespace.Services2")
                        })
                },
                @"using System;
using System.Linq;
using Namespace;
using Namespace.Resolvers1;
using Namespace.Services1;
using Namespace.Resolvers2;
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
            
            var resolverResolvedParameter1 = new SomeResolver1(someResolver1RandomService1);
            
            var resolverResolvedParameter2 = new SomeResolver2(someResolver2RandomService2);
            
            var target = new Destination
            {
                ResolvedParameter1 = resolverResolvedParameter1.Resolve(self.Parameter1),
                ResolvedParameter2 = resolverResolvedParameter2.Resolve(self.Parameter2),
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
                    new CollectionToCollectionPropertyMapping("Collection", false, "Namespace.Collections", "Collection", true, "CollectionItem", "Namespace.Collections", DestinationCollectionType.List)
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
                Collection = self.Collection.Select(element => element.MapToCollectionItem()).ToList(),
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
                    new CollectionToCollectionPropertyMapping("Collection", true, "Namespace.Collections", "Collection", false, "CollectionItem", "Namespace.Collections", DestinationCollectionType.Array)
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
                Collection = self?.Collection.Select(element => element.MapToCollectionItem()).ToArray() ?? Enumerable.Empty<CollectionItem>().ToArray(),
            };
            
            return target;
        }
    }
}
");

        // TODO: resolver arguments to nested mapper
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