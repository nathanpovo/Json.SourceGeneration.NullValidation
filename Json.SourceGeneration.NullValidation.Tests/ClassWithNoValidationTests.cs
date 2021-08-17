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
    }
}
