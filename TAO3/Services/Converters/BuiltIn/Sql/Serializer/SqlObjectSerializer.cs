using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

public record SqlConverterSettings(
    string? TableName);

public interface ISqlObjectSerializer : IObjectSerializer<SqlConverterSettings>
{

}

public class SqlObjectSerializer : ObjectSerializer<SqlConverterSettings>, ISqlObjectSerializer
{
    public SqlObjectSerializer()
    {
        AddBuiltIn();
    }

    public string Serialize(object? obj, int indentationLevel = 0, string indentationString = "    ")
    {
        return Serialize(obj, new SqlConverterSettings(null), indentationLevel, indentationString);
    }
    public void Serialize(StringBuilder sb, object? obj, int indentationLevel = 0, string indentationString = "    ")
    {
        Serialize(sb, obj, new SqlConverterSettings(null), indentationLevel, indentationString);
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
        
        AddConverter<Type, TypeTypeConverter>();

        AddGenericConverter(typeof(CollectionInitializerTypeConverter<>));
        AddGenericConverter(typeof(NullableTypeConverter<>));

        AddConverter<Enum, EnumTypeConverter>();
        AddConverter<object, ObjectTypeConverter>();
    }
}
