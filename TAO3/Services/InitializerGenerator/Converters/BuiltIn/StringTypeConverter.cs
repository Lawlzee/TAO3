using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.InitializerGenerator.Converters
{
    internal class StringTypeConverter : TypeConverter<string>
    {
        public override bool Convert(StringBuilder sb, string str, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("@\"");
            sb.Append(str.Replace("\"", "\"\""));
            sb.Append("\"");

            return true;
        }
    }
}
