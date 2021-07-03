using System;
using System.Collections.Generic;
using System.Text;
using TAO3.Internal.Types;

namespace TAO3.InitializerGenerator.Converters
{
    internal class NumberTypeConverter<T> : TypeConverter<T>
    {
        public override bool Convert(StringBuilder sb, T obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append(obj!.ToString());
            string? suffix = NumberHelper.GetNumberSuffix(typeof(T));
            if (suffix != null)
            {
                sb.Append(suffix);
            }

            return true;
        }
    }
}
