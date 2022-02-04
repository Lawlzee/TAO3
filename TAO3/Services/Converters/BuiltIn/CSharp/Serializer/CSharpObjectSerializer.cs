using Microsoft.CodeAnalysis;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

public record CSharpSerializerSettings();

public interface ICSharpObjectSerializer : IObjectSerializer<CSharpSerializerSettings>
{

}

public class CSharpObjectSerializer : ObjectSerializer<CSharpSerializerSettings>, ICSharpObjectSerializer
{
    public CSharpObjectSerializer()
    {
        AddBuiltIn();
    }

    public string Serialize(object? obj, int indentationLevel = 0, string indentationString = "    ")
    {
        return Serialize(obj, new CSharpSerializerSettings(), indentationLevel, indentationString);
    }
    public void Serialize(StringBuilder sb, object? obj, int indentationLevel = 0, string indentationString = "    ")
    {
        Serialize(sb, obj, new CSharpSerializerSettings(), indentationLevel, indentationString);
    }

    private void AddBuiltIn()
    {
        AddConverter<byte, NumberTypeConverter<byte>>();
        AddConverter<decimal, NumberTypeConverter<decimal>>();
        AddConverter<double, NumberTypeConverter<double>>();
        AddConverter<short, NumberTypeConverter<short>>();
        AddConverter<int, NumberTypeConverter<int>>();
        AddConverter<long, NumberTypeConverter<long>>();
        AddConverter<sbyte, NumberTypeConverter<sbyte>>();
        AddConverter<ushort, NumberTypeConverter<ushort>>();
        AddConverter<uint, NumberTypeConverter<uint>>();
        AddConverter<ulong, NumberTypeConverter<ulong>>();
        AddConverter<float, NumberTypeConverter<float>>();

        AddConverter<bool, BoolTypeConverter>();
        AddConverter<char, CharTypeConverter>();
        AddConverter<string, StringTypeConverter>();
        AddConverter<DateTime, DateTimeTypeConverter>();
        AddConverter<Guid, GuidTypeConverter>();
        
        AddConverter<ICSharpNode, ICSharpNodeTypeConverter>();
        AddConverter<SyntaxNode, SyntaxNodeTypeConverter>();

        AddGenericConverter(typeof(ValueTupleTypeConverter<,>));
        AddGenericConverter(typeof(ValueTupleTypeConverter<,,>));
        AddGenericConverter(typeof(ValueTupleTypeConverter<,,,>));
        AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,>));
        AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,,>));
        AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,,,>));
        AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,,,,>));

        AddGenericConverter(typeof(CollectionInitializerTypeConverter<>));
        AddGenericConverter(typeof(DictionaryInitializerTypeConverter<,>));
        
        AddGenericConverter(typeof(NullableTypeConverter<>));

        AddConverter<Enum, EnumTypeConverter>();
        AddConverter<object, AnonymousTypeConverter>();
        AddConverter<object, ObjectTypeConverter>();
    }
}
