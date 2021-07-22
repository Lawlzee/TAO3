using System;
using System.Collections.Generic;
using System.Text;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp
{
    internal class CharTypeConverter : TypeConverter<char>
    {
        public override bool Convert(StringBuilder sb, char obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append("'");
            //to do: escape caracteres
            sb.Append(obj);
            sb.Append("'");

            return true;
        }
    }
}
