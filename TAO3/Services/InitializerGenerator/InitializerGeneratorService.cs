using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.InitializerGenerator.Converters;
using TAO3.InitializerGenerator.Converters.Internal;
using TAO3.Internal.Types;

namespace TAO3.InitializerGenerator
{
    public interface IInitializerGeneratorService : IDisposable
    {
        InitializerGeneratorService AddConverter<T, TTypeConverter>() where TTypeConverter : TypeConverter<T>, new();
        InitializerGeneratorService AddConverter<T>(TypeConverter<T> typeConverter);
        InitializerGeneratorService AddGenericConverter(Type genericTypeConverterType);
        string Generate(object? obj, int indentationLevel = 0, string indentationString = "    ");
        void Generate(StringBuilder sb, object? obj, int indentationLevel = 0, string indentationString = "    ");
    }

    public class InitializerGeneratorService : IInitializerGeneratorService
    {
        private readonly Dictionary<Type, List<ITypeConverter>> _converters;

        public InitializerGeneratorService()
        {
            _converters = new Dictionary<Type, List<ITypeConverter>>();
            AddBuiltIn();
        }
        private InitializerGeneratorService AddBuiltIn()
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

            AddGenericConverter(typeof(ValueTupleTypeConverter<,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,,,,>));

            AddGenericConverter(typeof(CollectionInitializerTypeConverter<>));
            AddGenericConverter(typeof(DictionaryInitializerTypeConverter<,>));

            AddConverter<object, ObjectTypeConverter>();

            return this;
        }

        public InitializerGeneratorService AddConverter<T, TTypeConverter>()
            where TTypeConverter : TypeConverter<T>, new()
        {
            return AddConverter(new TTypeConverter());
        }

        public InitializerGeneratorService AddConverter<T>(TypeConverter<T> typeConverter)
        {
            DoAddConverter(typeof(T), typeConverter);
            return this;
        }

        public InitializerGeneratorService AddGenericConverter(Type genericTypeConverterType)
        {
            GenericTypeConverter typeConverter = new GenericTypeConverter(genericTypeConverterType);
            DoAddConverter(typeConverter.TypeToConvert.GetGenericTypeDefinition(), typeConverter);
            return this;
        }

        private void DoAddConverter(Type type, ITypeConverter typeConverter)
        {
            if (!_converters.ContainsKey(type))
            {
                _converters[type] = new List<ITypeConverter>();
            }

            _converters[type].Add(typeConverter);
        }

        public string Generate(object? obj, int indentationLevel = 0, string indentationString = "    ")
        {
            StringBuilder sb = new StringBuilder();
            Generate(sb, obj, new InitializerGeneratorOptions(indentationLevel, indentationString));
            return sb.ToString();
        }

        public void Generate(StringBuilder sb, object? obj, int indentationLevel = 0, string indentationString = "    ")
        {
            Generate(sb, obj, new InitializerGeneratorOptions(indentationLevel, indentationString));
        }

        internal void Generate(StringBuilder sb, object? obj, InitializerGeneratorOptions options)
        {
            if (obj == null)
            {
                sb.Append("null");
                return;
            }

            Type type = obj.GetType();

            while (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            //todo: handle flag enums
            if (type.IsEnum)
            {
                if (Enum.IsDefined(type, obj))
                {
                    sb.Append(type.Name);
                    sb.Append(".");
                    sb.Append(obj.ToString());
                    return;
                }

                sb.Append("(");
                sb.Append(type.PrettyPrint());
                sb.Append(")");
                Generate(sb, Convert.ChangeType(obj, type.GetEnumUnderlyingType()), options);
                return;
            }

            if (type.IsAnonymous())
            {
                new AnonymousTypeConverter().Convert(sb, obj, obj.GetType(), this, options);
                return;
            }

            foreach (Type parentType in type.GetParentTypes().Distinct())
            {
                bool success = TryConvert(sb, obj, options, parentType, parentType)
                    || (parentType.IsGenericType && TryConvert(sb, obj, options, parentType.GetGenericTypeDefinition(), parentType));

                if (success)
                {
                    return;
                }
            }
            throw new Exception($"Type {type.Name} is not supported");
        }

        private bool TryConvert(StringBuilder sb, object obj, InitializerGeneratorOptions options, Type type, Type rawType)
        {
            if (_converters.ContainsKey(type))
            {
                foreach (ITypeConverter converter in _converters[type])
                {
                    bool success = converter.Convert(sb, obj, rawType, this, options);
                    if (success)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Dispose()
        {
            foreach (ITypeConverter converter in _converters.Values.SelectMany(x => x))
            {
                converter.Dispose();
            }

            _converters.Clear();
        }
    }
}
