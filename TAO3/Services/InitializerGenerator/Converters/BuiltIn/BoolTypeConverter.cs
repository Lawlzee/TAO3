using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.InitializerGenerator.Converters
{
    internal class BoolTypeConverter : TypeConverter<bool>
    {
        public override bool Convert(StringBuilder sb, bool obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append(obj ? "true" : "false");
            return true;
        }
    }
}
