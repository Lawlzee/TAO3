using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.InitializerGenerator.Converters
{
    internal class CharTypeConverter : TypeConverter<char>
    {
        public override bool Convert(StringBuilder sb, char obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("'");
            //to do: escape caracteres
            sb.Append(obj);
            sb.Append("'");

            return true;
        }
    }
}
