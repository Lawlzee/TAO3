using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class CollectionInitializerTypeConverter<T> : TypeConverter<IEnumerable<T>, SqlConverterSettings>
{
    public override bool Convert(IEnumerable<T> obj, ObjectSerializerContext<SqlConverterSettings> context)
    {
        bool isFirst = true;
        foreach (T element in obj)
        {
            if (!isFirst)
            {
                context.AppendLine();
            }

            context.AppendIndentation();
            context.Serialize(element);

            isFirst = false;
        }
        return true;
    }
}
