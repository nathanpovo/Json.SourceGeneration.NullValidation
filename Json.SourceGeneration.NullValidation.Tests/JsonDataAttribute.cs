using Xunit;

namespace Json.SourceGeneration.NullValidation.Tests
{
    public class TestData
    {
        public const string EmptyJson = "{}";

        public const string AllValuesJson = @"
{
    ""ReferenceProperty"":""value"",
    ""NullableReferenceProperty"":""value"",
    ""NullableValueProperty"":1234,
    ""NullableStructProperty"":""2021-08-15T00:00:00""
}";

        public const string NullReferencePropertyJson = @"
{
    ""ReferenceProperty"":null,
    ""NullableReferenceProperty"":""value"",
    ""NullableValueProperty"":1234,
    ""NullableStructProperty"":""2021-08-15T00:00:00""
}";

        public const string NullReferenceFieldJson = @"
{
    ""ReferenceProperty"":""value"",
    ""NullableReferenceProperty"":null,
    ""NullableValueProperty"":1234,
    ""NullableStructProperty"":""2021-08-15T00:00:00""
}";

        public const string NullValueFieldJson = @"
{
    ""ReferenceProperty"":""value"",
    ""NullableReferenceProperty"":""value"",
    ""NullableValueProperty"":null,
    ""NullableStructProperty"":""2021-08-15T00:00:00""
}";

        public const string NullStructFieldJson = @"
{
    ""ReferenceProperty"":""value"",
    ""NullableReferenceProperty"":""value"",
    ""NullableValueProperty"":1234,
    ""NullableStructProperty"":null
}";

        public static TheoryData<string> InvalidJsonData => new TheoryData<string>
        {
            EmptyJson,
            NullReferenceFieldJson,
            NullReferencePropertyJson,
            NullValueFieldJson,
            NullStructFieldJson
        };
    }
}
