using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.InitializerGenerator.Converters.Internal
{
    internal class ToStringTypeConverter<T> : TypeConverter<T>
    {
        public override bool Convert(StringBuilder sb, T obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append(obj!.ToString());

            return true;
        }
    }
}
