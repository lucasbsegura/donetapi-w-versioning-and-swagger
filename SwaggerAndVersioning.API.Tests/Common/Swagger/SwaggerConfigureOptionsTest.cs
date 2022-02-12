using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using NSubstitute;
using SwaggerAndVersioning.API.Common.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SwaggerAndVersioning.API.Tests.Common.Swagger
{
    public class SwaggerConfigureOptionsTest
    {
        private IConfigureOptions<SwaggerGenOptions> _sut;

        private readonly SwaggerGenOptions _options;

        private readonly string GroupName = "v1";
        private readonly string GroupNameV2 = "v2";
        private readonly int ApiMinorVersion = 0;
        private readonly int ApiMajorVersion = 1;
        private readonly int ApiV2MajorVersion = 2;
        private readonly string ApiVersion = "1.0";
        private readonly string ApiVersion2 = "2.0";
        private readonly string Title = "Swagger And Versioning";
        private readonly string DeprecatedDescription = " This API version has been deprecated.";


        public SwaggerConfigureOptionsTest()
        {
            _options = Substitute.For<SwaggerGenOptions>();
        }

        [Fact]
        public void Configure_ShouldSetConfigurationKey()
        {
            var provider = MockApiVersionDescriptionProvider(false);
            _sut = new SwaggerConfigureOptions(provider);

            _sut.Configure(_options);

            _options.SwaggerGeneratorOptions.SwaggerDocs.First().Key.Should().Be(GroupName);
        }

        [Fact]
        public void Configure_ShouldSetTitle()
        {
            var provider = MockApiVersionDescriptionProvider(false);
            _sut = new SwaggerConfigureOptions(provider);

            _sut.Configure(_options);

            _options.SwaggerGeneratorOptions.SwaggerDocs.First().Value.Title.Should().Be(Title);
        }

        [Fact]
        public void Configure_ShouldGetVersion()
        {
            var provider = MockApiVersionDescriptionProvider(false);
            _sut = new SwaggerConfigureOptions(provider);

            _sut.Configure(_options);

            _options.SwaggerGeneratorOptions.SwaggerDocs.First().Value.Version.Should().Be(ApiVersion);
        }

        [Fact]
        public void Configure_ShouldSetDescription_WhenApiVersionIsDeprecated()
        {
            var provider = MockApiVersionDescriptionProvider(true);
            _sut = new SwaggerConfigureOptions(provider);

            _sut.Configure(_options);

            _options.SwaggerGeneratorOptions.SwaggerDocs.First().Value.Description.Should().Be(DeprecatedDescription);
        }

        [Fact]
        public void Configure_TwoVersionsOfApi_ShouldGetVersionForEachApi()
        {
            var provider = MockApiVersionDescriptionProvider2Versions();
            _sut = new SwaggerConfigureOptions(provider);

            _sut.Configure(_options);

            _options.SwaggerGeneratorOptions.SwaggerDocs["v1"].Version.Should().Be(ApiVersion);
            _options.SwaggerGeneratorOptions.SwaggerDocs["v2"].Version.Should().Be(ApiVersion2);
        }

        [Fact]
        public void Configure_TwoVersionsOfApi_V1ShouldBeDeprecated()
        {
            var provider = MockApiVersionDescriptionProvider2Versions();
            _sut = new SwaggerConfigureOptions(provider);

            _sut.Configure(_options);

            _options.SwaggerGeneratorOptions.SwaggerDocs["v1"].Description.Should().Be(DeprecatedDescription);
            _options.SwaggerGeneratorOptions.SwaggerDocs["v2"].Description.Should().BeNull();
        }

        private IApiVersionDescriptionProvider MockApiVersionDescriptionProvider(bool isDeprecated)
        {
            var provider = Substitute.For<IApiVersionDescriptionProvider>();
            provider.ApiVersionDescriptions.Returns(new List<ApiVersionDescription>
            {
                new ApiVersionDescription(new ApiVersion(ApiMajorVersion, ApiMinorVersion), GroupName, isDeprecated)
            });
            return provider;
        }

        private IApiVersionDescriptionProvider MockApiVersionDescriptionProvider2Versions()
        {
            var provider = Substitute.For<IApiVersionDescriptionProvider>();
            provider.ApiVersionDescriptions.Returns(new List<ApiVersionDescription>
            {
                new ApiVersionDescription(new ApiVersion(ApiMajorVersion, ApiMinorVersion), GroupName, true),
                new ApiVersionDescription(new ApiVersion(ApiV2MajorVersion, ApiMinorVersion), GroupNameV2, false)
            });
            return provider;
        }
    }
}
