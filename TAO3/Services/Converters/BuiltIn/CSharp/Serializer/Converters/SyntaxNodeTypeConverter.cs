using Microsoft.CodeAnalysis;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class SyntaxNodeTypeConverter : TypeConverter<SyntaxNode, CSharpSerializerSettings>
{
    public override bool Convert(SyntaxNode obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append(obj.ToString());
        return true;
    }
}
