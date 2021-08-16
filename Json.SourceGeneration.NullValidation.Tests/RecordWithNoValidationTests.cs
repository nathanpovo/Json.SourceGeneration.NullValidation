using System;
using System.Text.Json;
using FluentAssertions;
using Json.SourceGeneration.NullValidation.Tests.Models;
using Xunit;

namespace Json.SourceGeneration.NullValidation.Tests
{
    public class RecordWithNoValidationTests
    {
        [Theory]
        [MemberData(nameof(TestData.InvalidJsonData), MemberType = typeof(TestData))]
        public void WhenRecordIsNotValidatedShouldNotThrow(string json)
        {
            Action action = () => JsonSerializer.Deserialize<TypicalRecord>(json);

            action.Should().NotThrow("validation is not performed during deserialization");
        }

        [Fact]
        public void WhenReferencePropertyIsNullShouldNotThrow()
        {
#nullable disable
            TypicalRecord typicalRecord = new(null, "value");
#nullable restore

            Func<string> action = () => JsonSerializer.Serialize(typicalRecord);

            action
                .Should()
                .NotThrow("validation is not performed during serialization");
        }
    }
}
