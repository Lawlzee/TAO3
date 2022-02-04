namespace TAO3.TextSerializer;

internal interface ITypeConverter : IDisposable
{
    bool Convert(StringBuilder sb, object obj, Type objectType, ObjectSerializer serializer, ObjectSerializerOptions options);

}

public abstract class TypeConverter<T> : ITypeConverter
{
    public bool Convert(StringBuilder sb, object obj, Type objectType, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        if (obj is T x && typeof(T) == objectType)
        {
            return Convert(sb, x, serializer, options);
        }

        return false;
    }

    public abstract bool Convert(StringBuilder sb, T obj, ObjectSerializer serializer, ObjectSerializerOptions options);

    public virtual void Dispose()
    {
        
    }
}
