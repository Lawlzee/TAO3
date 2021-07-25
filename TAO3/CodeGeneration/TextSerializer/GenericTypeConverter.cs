using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAO3.TextSerializer
{
    internal class GenericTypeConverter : ITypeConverter
    {
        private readonly Dictionary<Type, ITypeConverter> _convertersCache = new Dictionary<Type, ITypeConverter>();

        public Type TypeToConvert { get; }

        private readonly Type _typeConverterType;

        public GenericTypeConverter(Type typeConverterType)
        {
            if (!typeConverterType.IsGenericTypeDefinition
                || typeConverterType.BaseType == null
                || !typeConverterType.BaseType.IsGenericType
                || typeConverterType.BaseType.GetGenericTypeDefinition() != typeof(TypeConverter<>))
            {
                throw new ArgumentException(nameof(typeConverterType));
            }

            TypeToConvert = typeConverterType.BaseType.GetGenericArguments()[0];

            if (!TypeToConvert.IsGenericType)
            {
                throw new Exception("The type to convert must be a generic type");
            }

            _convertersCache = new Dictionary<Type, ITypeConverter>();
            _typeConverterType = typeConverterType;
        }

        public bool Convert(StringBuilder sb, object obj, Type objectType, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            if (!objectType.IsGenericType
                || objectType.GetGenericTypeDefinition() != TypeToConvert.GetGenericTypeDefinition())
            {
                return false;
            }

            //to do: add validation type constaints

            ITypeConverter? cachedTypeConverter = _convertersCache.GetValueOrDefault(objectType);
            if (cachedTypeConverter != null)
            {
                return cachedTypeConverter.Convert(sb, obj, objectType, serializer, options);
            }

            Type typeConverterType = CreateConcreteTypeConverterType(objectType);
            ITypeConverter typeConverter = (ITypeConverter)Activator.CreateInstance(typeConverterType)!;
            _convertersCache[objectType] = typeConverter;

            return typeConverter.Convert(sb, obj, objectType, serializer, options);
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
}
