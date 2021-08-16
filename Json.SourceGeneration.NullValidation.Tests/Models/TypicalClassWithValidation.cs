using System;
using System.Text.Json.Serialization;

namespace Json.SourceGeneration.NullValidation.Tests.Models
{
    [ValidateNullability]
    public partial class TypicalClassWithValidation
    {
        [JsonInclude]
        public string ReferenceField;

        [JsonInclude]
        public string? NullableReferenceField;

        [JsonInclude]
        public long? NullableValueField;

        [JsonInclude]
        public DateTime? NullableStructField;

        public TypicalClassWithValidation(string referenceProperty, string referenceField)
        {
            ReferenceProperty = referenceProperty;
            ReferenceField = referenceField;
        }

        public string ReferenceProperty { get; set; }

        public string? NullableReferenceProperty { get; set; }
        public long? NullableValueProperty { get; set; }
        public DateTime? NullableStructProperty { get; set; }
    }
}
