using System;
using System.Text.Json;
using FluentAssertions;
using Json.SourceGeneration.NullValidation.Tests.Models;
using Xunit;

namespace Json.SourceGeneration.NullValidation.Tests
{
    public class ClassWithNoValidationTests
    {
        [Theory]
        [MemberData(nameof(TestData.InvalidJsonData), MemberType = typeof(TestData))]
        public void WhenClassIsNotValidatedShouldNotThrow(string json)
        {
            Action action = () => JsonSerializer.Deserialize<TypicalClass>(json);

            action.Should().NotThrow("validation is not performed during deserialization");
        }

        [Fact]
        public void WhenReferencePropertyIsNullShouldNotThrow()
        {
#nullable disable
            TypicalClass typicalClass = new(null, "value");
#nullable restore

            Func<string> action = () => JsonSerializer.Serialize(typicalClass);

            action
                .Should()
                .NotThrow("validation is not performed during serialization");
        }
    }
}
