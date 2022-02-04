using Newtonsoft.Json;
using TAO3.TypeProvider;

namespace TAO3.Converters.Json;

internal class JsonPropertyAnnotator : IPropertyAnnotator
{
    public void Annotate(ClassPropertySchema property, PropertyAnnotatorContext context)
    {
        context.Using(typeof(JsonPropertyAttribute).Namespace!);
        context.StringBuilder.Append($@"[JsonProperty(""{property.FullName.Replace("\"", "\"\"")}"")]
    ");
    }
}
