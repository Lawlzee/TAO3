using CsvHelper.Configuration.Attributes;
using TAO3.TypeProvider;

namespace TAO3.Converters.Csv;

internal class CsvIndexAnnotator : IPropertyAnnotator
{
    public void Annotate(ClassPropertySchema property, PropertyAnnotatorContext context)
    {
        context.Using(typeof(IndexAttribute).Namespace!);
        context.StringBuilder.Append($@"[Index({context.Index})]
    ");
    }
}
