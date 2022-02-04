using Newtonsoft.Json;
using TAO3.TypeProvider;

namespace TAO3.Converters.Xml;

internal class ValueToListAnnotator : IPropertyAnnotator
{
    private IDomSchemaSerializer _serializer;

    public ValueToListAnnotator(IDomSchemaSerializer serializer)
    {
        _serializer = serializer;
    }

    public void Annotate(ClassPropertySchema property, PropertyAnnotatorContext context)
    {
        if (property.Type.Type is CollectionTypeSchema collection)
        {
            context.Using(typeof(JsonConverterAttribute).Namespace!);
            context.Using(typeof(ValueToList<>).Namespace!);
            context.StringBuilder.Append($@"[JsonConverter(typeof(ValueToList<{_serializer.PrettyPrint(collection.InnerType)}>))]
    ");
        }
    }
}
