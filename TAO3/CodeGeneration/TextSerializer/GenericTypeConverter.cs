namespace TAO3.TextSerializer;

internal class GenericTypeConverter<TSettings> : ITypeConverter<TSettings>
{
    private readonly Dictionary<Type, ITypeConverter<TSettings>> _convertersCache = new Dictionary<Type, ITypeConverter<TSettings>>();

    public Type TypeToConvert { get; }

    private readonly Type _typeConverterType;

    public GenericTypeConverter(Type typeConverterType)
    {
        if (!typeConverterType.IsGenericTypeDefinition
            || typeConverterType.BaseType == null
            || !typeConverterType.BaseType.IsGenericType
            || typeConverterType.BaseType.GetGenericTypeDefinition() != typeof(TypeConverter<,>))
        {
            throw new ArgumentException(nameof(typeConverterType));
        }

        TypeToConvert = typeConverterType.BaseType.GetGenericArguments()[0];

        if (!TypeToConvert.IsGenericType)
        {
            throw new Exception("The type to convert must be a generic type");
        }

        if (typeConverterType.BaseType.GetGenericArguments()[1] != typeof(TSettings))
        {
            throw new Exception("Invalid TSettings type");
        }

        _convertersCache = new Dictionary<Type, ITypeConverter<TSettings>>();
        _typeConverterType = typeConverterType;
    }

    public bool Convert(object obj, Type objectType, ObjectSerializerContext<TSettings> context)
    {
        if (!objectType.IsGenericType
            || objectType.GetGenericTypeDefinition() != TypeToConvert.GetGenericTypeDefinition())
        {
            return false;
        }

        //to do: add validation type constaints

        ITypeConverter<TSettings>? cachedTypeConverter = _convertersCache.GetValueOrDefault(objectType);
        if (cachedTypeConverter != null)
        {
            return cachedTypeConverter.Convert(obj, objectType, context);
        }

        Type typeConverterType = CreateConcreteTypeConverterType(objectType);
        ITypeConverter<TSettings> typeConverter = (ITypeConverter<TSettings>)Activator.CreateInstance(typeConverterType)!;
        _convertersCache[objectType] = typeConverter;

        return typeConverter.Convert(obj, objectType, context);
    }

    //to do: support nested types. Ex A<B<C>, D>
    private Type CreateConcreteTypeConverterType(Type objectType)
    {
        Dictionary<Type, Type> concreteTypeArgByTypeParam = TypeToConvert.GetGenericArguments()
            .Zip(
                objectType.GetGenericArguments(),
                (typeParam, typeArg) => new
                {
                    Key = typeParam,
                    Value = typeArg
                })
            .ToDictionary(x => x.Key, x => x.Value);

        Type[] typeArguments = _typeConverterType
            .GetGenericArguments()
            .Select(x => concreteTypeArgByTypeParam[x])
            .ToArray();

        return _typeConverterType.MakeGenericType(typeArguments);
    }

    public void Dispose()
    {
        _convertersCache.Clear();
    }
}
