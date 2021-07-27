using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TAO3.TypeProvider;

namespace TAO3.Converters.Xml
{
    internal class ValueToListAnnotator : IPropertyAnnotator
    {
        private IDomSchemaSerializer _serializer;

        public ValueToListAnnotator(IDomSchemaSerializer serializer)
        {
            _serializer = serializer;
        }

        public void Annotate(ClassPropertySchema property, PropertyAnnotatorContext context)
        {
            if (property.Type.Type is CollectionTypeSchema)
            {
                context.Using(typeof(JsonConverterAttribute).Namespace!);
                context.Using(typeof(ValueToList<>).Namespace!);
                context.StringBuilder.Append($@"[JsonConverter(typeof(ValueToList<{_serializer.PrettyPrint(property.Type)}>)]
    ");
            }
        }
    }
}
