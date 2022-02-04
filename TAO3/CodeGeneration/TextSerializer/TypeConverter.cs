namespace TAO3.TextSerializer;

internal interface ITypeConverter<TSettings> : IDisposable
{
    bool Convert(object obj, Type objectType, ObjectSerializerContext<TSettings> context);

}

public abstract class TypeConverter<T, TSettings> : ITypeConverter<TSettings>
{
    public bool Convert(object obj, Type objectType, ObjectSerializerContext<TSettings> context)
    {
        if (obj is T x && typeof(T) == objectType)
        {
            return Convert(x, context);
        }

        return false;
    }

    public abstract bool Convert(T obj, ObjectSerializerContext<TSettings> context);

    public virtual void Dispose()
    {
        
    }
}
