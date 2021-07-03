using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.InitializerGenerator.Converters
{
    internal class GuidTypeConverter : TypeConverter<Guid>
    {
        public override bool Convert(StringBuilder sb, Guid obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append($"new Guid(\"{obj}\")");

            return true;
        }
    }
}
