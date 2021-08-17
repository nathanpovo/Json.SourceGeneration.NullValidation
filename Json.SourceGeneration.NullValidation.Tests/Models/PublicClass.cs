using System.Text.Json.Serialization;

namespace Json.SourceGeneration.NullValidation.Tests.Models
{
    [ValidateNullability]
    public partial class PublicClass
    {
        public PublicClass(string property)
            => Property = property;

        public string Property { get; set; }
    }
}
