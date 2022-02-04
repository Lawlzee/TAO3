using TAO3.Internal.Types;

namespace TAO3.TextSerializer;

public interface IObjectSerializer<TSettings> : IDisposable
{
    IObjectSerializer<TSettings> AddConverter<T, TTypeConverter>(int priority = 0) where TTypeConverter : TypeConverter<T, TSettings>, new();
    IObjectSerializer<TSettings> AddConverter<T>(TypeConverter<T, TSettings> typeConverter, int priority = 0);
    IObjectSerializer<TSettings> AddGenericConverter(Type genericTypeConverterType, int priority = 0);
    string Serialize(object? obj, TSettings settings, int indentationLevel = 0, string indentationString = "    ");
    void Serialize(StringBuilder sb, object? obj, TSettings settings, int indentationLevel = 0, string indentationString = "    ");
}

public class ObjectSerializer<TSettings> : IObjectSerializer<TSettings>
{
    private readonly Dictionary<Type, List<PriorisedTypeConverter>> _converters;

    public ObjectSerializer()
    {
        _converters = new Dictionary<Type, List<PriorisedTypeConverter>>();
    }

    public IObjectSerializer<TSettings> AddConverter<T, TTypeConverter>(int priority = 0)
        where TTypeConverter : TypeConverter<T, TSettings>, new()
    {
        return AddConverter(new TTypeConverter(), priority);
    }

    public IObjectSerializer<TSettings> AddConverter<T>(TypeConverter<T, TSettings> typeConverter, int priority = 0)
    {
        DoAddConverter(typeof(T), typeConverter, priority);
        return this;
    }

    public IObjectSerializer<TSettings> AddGenericConverter(Type genericTypeConverterType, int priority = 0)
    {
        GenericTypeConverter<TSettings> typeConverter = new GenericTypeConverter<TSettings>(genericTypeConverterType);
        DoAddConverter(typeConverter.TypeToConvert.GetGenericTypeDefinition(), typeConverter, priority);
        return this;
    }

    private void DoAddConverter(Type type, ITypeConverter<TSettings> typeConverter, int priority)
    {
        if (!_converters.ContainsKey(type))
        {
            _converters[type] = new List<PriorisedTypeConverter>();
        }

        _converters[type].Add(new PriorisedTypeConverter(typeConverter, priority));
    }

    public string Serialize(object? obj, TSettings settings, int indentationLevel = 0, string indentationString = "    ")
    {
        StringBuilder sb = new StringBuilder();
        Serialize(obj, new ObjectSerializerContext<TSettings>(indentationLevel, indentationString, settings, sb, this));
        return sb.ToString();
    }

    public void Serialize(StringBuilder sb, object? obj, TSettings settings, int indentationLevel = 0, string indentationString = "    ")
    {
        Serialize(obj, new ObjectSerializerContext<TSettings>(indentationLevel, indentationString, settings, sb, this));
    }

    internal void Serialize(object? obj, ObjectSerializerContext<TSettings> context)
    {
        if (obj == null)
        {
            SerializeNull(this, context);
            return;
        }

        Type type = obj.GetType();

        foreach (Type parentType in type.GetSelfAndParentTypes())
        {
            bool success = TryConvert(obj, context, parentType, parentType)
                || (parentType.IsGenericType && TryConvert(obj, context, parentType.GetGenericTypeDefinition(), parentType));

            if (success)
            {
                return;
            }
        }
        throw new Exception($"Type {type.Name} is not supported");
    }

    private bool TryConvert(object obj, ObjectSerializerContext<TSettings> settings, Type type, Type rawType)
    {
        if (_converters.ContainsKey(type))
        {
            IEnumerable<ITypeConverter<TSettings>> sortedTypeConverters = _converters[type]
                .OrderByDescending(x => x.Priority)
                .Select(x => x.TypeConverter);

            foreach (ITypeConverter<TSettings> converter in sortedTypeConverters)
            {
                bool success = converter.Convert(obj, rawType, settings);
                if (success)
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected virtual void SerializeNull(ObjectSerializer<TSettings> serializer, ObjectSerializerContext<TSettings> context)
    {
        context.Append("null");
    }

    public virtual void Dispose()
    {
        foreach (ITypeConverter<TSettings> converter in _converters.Values.SelectMany(x => x))
        {
            converter.Dispose();
        }

        _converters.Clear();
    }

    internal record PriorisedTypeConverter(
        ITypeConverter<TSettings> TypeConverter,
        int Priority);
}
