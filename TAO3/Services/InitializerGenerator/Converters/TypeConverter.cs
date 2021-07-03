using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.InitializerGenerator.Converters
{
    public interface ITypeConverter : IDisposable
    {
        bool Convert(StringBuilder sb, object obj, Type objectType, InitializerGeneratorService generator, InitializerGeneratorOptions options);

    }

    public abstract class TypeConverter<T> : ITypeConverter
    {
        public bool Convert(StringBuilder sb, object obj, Type objectType, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            if (obj is T x && typeof(T) == objectType)
            {
                return Convert(sb, x, generator, options);
            }

            return false;
        }

        public abstract bool Convert(StringBuilder sb, T obj, InitializerGeneratorService generator, InitializerGeneratorOptions options);

        public virtual void Dispose()
        {
            
        }
    }
}
