using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Example.Records;
using Example.Sources;

[assembly: GeneratedMapper.Attributes.MapperGeneratorConfiguration(
    ThrowWhenNotNullableElementIsNull = true, 
    ThrowWhenNotNullablePropertyIsNull = true,
    GenerateEnumerableMethods = true,
    GenerateExpressions = true)]
namespace Example
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var source = new Source
            {
                Name = "SourceName",
                Company = new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "SourceCompanyName",
                    Revenue = 1234567,
                    StartDate = new DateTime(2020, 1, 1),
                    SubCompanies = new List<Company?>
                    {
                        new Company { Id = Guid.NewGuid(), Name = "SubCompany1", Revenue = 12, StartDate = new DateTime(2020, 2, 1) },
                        default,
                        new Company { Id = Guid.NewGuid(), Name = "SubCompany2", Revenue = 1234, StartDate = new DateTime(2020, 3, 1), SubCompanies = new List<Company?>{
                            new Company { Id = Guid.NewGuid(), Name = "SubCompany2a", Revenue = 123, StartDate = new DateTime(2020, 4, 1) },
                            default,
                            new Company { Id = Guid.NewGuid(), Name = "SubCompany2b", Revenue = 234, StartDate = new DateTime(2020, 5, 1) },
                        }},
                        default
                    }
                },
                Metadata = new[] {
                    new Source.SourceMetadata
                    {
                        Data = Guid.NewGuid().ToString()
                    }},
                MetadataDictionary = new Dictionary<string, Source.SourceMetadata>
                {
                    { "tag1", new Source.SourceMetadata { Data = "tag1 metadata "} },
                    { "tag2", new Source.SourceMetadata { Data = "tag2 metadata "} },
                }
            };

            // important known limitation for source generators:

            // if this example is run inside the GeneratorMapper solution, intellisense will fail to 
            //      load the GeneratorMapper.dll so these two methods will be flagged as missing --
            //      the project still builds and runs correctly. 
            //      rebuild everything and restart VS will solve it, but a new build of the generator will not
            //      be automatically loaded in VS.

            // if this example is run with GeneratorMapper as NuGet package, intellisense will work correctly

            var options = new JsonSerializerOptions { WriteIndented = true };

            Console.WriteLine(JsonSerializer.Serialize(source.MapToSimpleDestination(), options));
            Console.WriteLine(JsonSerializer.Serialize(source.MapToComplexDestination(7, new[] { 1.2, 1.3 }, CultureInfo.CurrentCulture), options));

            var record = new TestRecord("Test");

            var destination = record.MapToTestRecordDestination();

            Console.WriteLine(JsonSerializer.Serialize(destination, options));

            // when GenerateEnumerableMethods == true, an extension method for easily mapping enumerations is also generated
            var destinations = new[] { record }.MapToTestRecordDestination();

            // when GenerateExpressions == true, an expression is also generated for easy mapping objects in EF for example
            // Resolvers are not supported and skipped, and the expression can contain instructions which cannot be parsed by EF / CosmosDB if it's too complicated.
            var destinationExpression = Sources.Expressions.Source.ToComplexDestination(7, new[] { 1.2, 1.3 }, CultureInfo.CurrentCulture);
            var destinationLambda = destinationExpression.Compile();

            var destinationViaExpression = destinationLambda.Invoke(source);

            Console.WriteLine(JsonSerializer.Serialize(destinationViaExpression, options));

            var companyExpression = Sources.Expressions.Company.ToCompanyDestination(1, new[] { 1.2, 1.3 }, CultureInfo.CurrentCulture);
            var companyLambda = companyExpression.Compile();

            var companyViaExpression = companyLambda.Invoke(source.Company);

            Console.WriteLine(JsonSerializer.Serialize(companyViaExpression, options));
        }
    }
}
