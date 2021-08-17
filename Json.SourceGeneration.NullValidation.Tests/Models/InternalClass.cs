using System.Text.Json.Serialization;

namespace Json.SourceGeneration.NullValidation.Tests.Models
{
    [ValidateNullability]
    public partial class InternalClass
    {
        public InternalClass(string property)
            => Property = property;

        public string Property { get; set; }
    }
}
