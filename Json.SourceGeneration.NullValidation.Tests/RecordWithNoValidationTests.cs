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
    }
}
