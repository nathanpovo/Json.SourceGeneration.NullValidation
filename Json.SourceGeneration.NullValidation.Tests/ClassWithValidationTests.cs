using System;
using System.Text.Json;
using FluentAssertions;
using Json.SourceGeneration.NullValidation.Tests.Models;
using Xunit;

namespace Json.SourceGeneration.NullValidation.Tests
{
    public class ClassWithValidationTests
    {
        [Theory]
        [MemberData(nameof(TestData.InvalidJsonData), MemberType = typeof(TestData))]
        public void WhenClassIsNotValidatedShouldThrow(string json)
        {
            Action action = () => JsonSerializer.Deserialize<TypicalClassWithValidation>(json);

            action.Should().NotThrow("validation is not performed during deserialization");
        }

        [Fact]
        public void WhenReferencePropertyIsNullShouldThrow()
        {
#nullable disable
            TypicalClassWithValidation typicalClass = new(null, "value");
#nullable restore

            Func<string> action = () => JsonSerializer.Serialize(typicalClass);

            action
                .Should()
                .NotThrow("validation is not performed during serialization");
        }
    }
}
