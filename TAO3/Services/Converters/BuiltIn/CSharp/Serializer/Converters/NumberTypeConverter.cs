using System;
using System.Collections.Generic;
using System.Text;
using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp
{
    internal class NumberTypeConverter<T> : TypeConverter<T>
    {
        public override bool Convert(StringBuilder sb, T obj, ObjectSerializer serializer, ObjectSerializerOptions options)
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
