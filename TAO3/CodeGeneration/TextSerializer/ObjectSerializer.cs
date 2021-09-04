using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Types;

namespace TAO3.TextSerializer
{
    public interface IObjectSerializer : IDisposable
    {
        IObjectSerializer AddConverter<T, TTypeConverter>(int priority = 0) where TTypeConverter : TypeConverter<T>, new();
        IObjectSerializer AddConverter<T>(TypeConverter<T> typeConverter, int priority = 0);
        IObjectSerializer AddGenericConverter(Type genericTypeConverterType, int priority = 0);
        string Serialize(object? obj, int indentationLevel = 0, string indentationString = "    ");
        void Serialize(StringBuilder sb, object? obj, int indentationLevel = 0, string indentationString = "    ");
    }

    public class ObjectSerializer : IObjectSerializer
    {
        private readonly Dictionary<Type, List<PriorisedTypeConverter>> _converters;

        public ObjectSerializer()
        {
            _converters = new Dictionary<Type, List<PriorisedTypeConverter>>();
        }

        public IObjectSerializer AddConverter<T, TTypeConverter>(int priority = 0)
            where TTypeConverter : TypeConverter<T>, new()
        {
            return AddConverter(new TTypeConverter(), priority);
        }

        public IObjectSerializer AddConverter<T>(TypeConverter<T> typeConverter, int priority = 0)
        {
            DoAddConverter(typeof(T), typeConverter, priority);
            return this;
        }

        public IObjectSerializer AddGenericConverter(Type genericTypeConverterType, int priority = 0)
        {
            GenericTypeConverter typeConverter = new GenericTypeConverter(genericTypeConverterType);
            DoAddConverter(typeConverter.TypeToConvert.GetGenericTypeDefinition(), typeConverter, priority);
            return this;
        }

        private void DoAddConverter(Type type, ITypeConverter typeConverter, int priority)
        {
            if (!_converters.ContainsKey(type))
            {
                _converters[type] = new List<PriorisedTypeConverter>();
            }

            _converters[type].Add(new PriorisedTypeConverter(typeConverter, priority));
        }

        public string Serialize(object? obj, int indentationLevel = 0, string indentationString = "    ")
        {
            StringBuilder sb = new StringBuilder();
            Serialize(sb, obj, new ObjectSerializerOptions(indentationLevel, indentationString));
            return sb.ToString();
        }

        public void Serialize(StringBuilder sb, object? obj, int indentationLevel = 0, string indentationString = "    ")
        {
            Serialize(sb, obj, new ObjectSerializerOptions(indentationLevel, indentationString));
        }

        internal void Serialize(StringBuilder sb, object? obj, ObjectSerializerOptions options)
        {
            if (obj == null)
            {
                SerializeNull(sb, this, options);
                return;
            }

            Type type = obj.GetType();

            foreach (Type parentType in type.GetSelfAndParentTypes())
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

        private bool TryConvert(StringBuilder sb, object obj, ObjectSerializerOptions options, Type type, Type rawType)
        {
            if (_converters.ContainsKey(type))
            {
                IEnumerable<ITypeConverter> sortedTypeConverters = _converters[type]
                    .OrderByDescending(x => x.Priority)
                    .Select(x => x.TypeConverter);

                foreach (ITypeConverter converter in sortedTypeConverters)
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

        protected virtual void SerializeNull(StringBuilder sb, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append("null");
        }

        public virtual void Dispose()
        {
            foreach (ITypeConverter converter in _converters.Values.SelectMany(x => x))
            {
                converter.Dispose();
            }

            _converters.Clear();
        }

        internal record PriorisedTypeConverter(
            ITypeConverter TypeConverter,
            int Priority);
    }
}
