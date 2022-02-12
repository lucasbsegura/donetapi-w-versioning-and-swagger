using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using NSubstitute;
using SwaggerAndVersioning.API.Common.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace SwaggerAndVersioning.API.Tests.Common.Swagger
{
    public class SwaggerDefaultValuesTest
    {
        private readonly IOperationFilter _sut;
        private readonly OpenApiOperation _operation;
        private OperationFilterContext _context;

        private readonly string Version = "v1";
        private readonly string Method = "GET";
        private readonly string RelativePath = "v1/duty";
        private readonly string Name = "ceqId";
        private readonly string Description = "description";

        public SwaggerDefaultValuesTest()
        {
            _sut = Substitute.For<SwaggerDefaultValues>();
            _operation = Substitute.For<OpenApiOperation>();
            MockOperationFilterContext();
        }

        [Fact]
        public void Apply_ApiDescription_ProportiesMustHaveValues()
        {
            _sut.Apply(_operation, _context);

            _context.ApiDescription.GroupName.Should().Be(Version);
            _context.ApiDescription.HttpMethod.Should().Be(Method);
            _context.ApiDescription.RelativePath.Should().Be(RelativePath);
        }

        [Fact]
        public void Apply_OperationParameters_NameAndDescriptionMustHaveValue()
        {
            _operation.Parameters = MockOperationParameters(true);
            _context.ApiDescription.ParameterDescriptions.Add(MockContextApiDescriptionParameterDescription());

            _sut.Apply(_operation, _context);

            _operation.Parameters.First().Name.Should().Be(Name);
            _operation.Parameters.First().Description.Should().Be(Description);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Apply_OperationParameters_RequiredFieldShouldMetSpecification(bool required)
        {
            _operation.Parameters = MockOperationParameters(required);
            _context.ApiDescription.ParameterDescriptions.Add(MockContextApiDescriptionParameterDescription());

            _sut.Apply(_operation, _context);

            _operation.Parameters.First().Required.Should().Be(required);
        }

        private void MockOperationFilterContext()
        {
            var apiDescriptoion = new ApiDescription
            {
                ActionDescriptor = Substitute.For<Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor>(),
                GroupName = Version,
                HttpMethod = Method,
                RelativePath = RelativePath
            };
            _context = new OperationFilterContext(apiDescriptoion,
                                                  Substitute.For<ISchemaGenerator>(),
                                                  Substitute.For<SchemaRepository>(),
                                                  Substitute.For<MethodInfo>());
        }

        private List<OpenApiParameter> MockOperationParameters(bool required) =>
            new()
            {
                new()
                {
                    Name = Name,
                    Description = null,
                    Required = required
                }
            };

        private ApiParameterDescription MockContextApiDescriptionParameterDescription() =>
            new()
            {
                Name = Name,
                ModelMetadata = new ApiVersionModelMetadata(Substitute.For<IModelMetadataProvider>(), Description)
            };

    }
}
