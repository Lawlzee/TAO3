using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.InitializerGenerator.Converters
{
    internal class DateTimeTypeConverter : TypeConverter<DateTime>
    {
        public override bool Convert(StringBuilder sb, DateTime date, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("new DateTime(");
            sb.Append(date.Year);
            sb.Append(", ");
            sb.Append(date.Month);
            sb.Append(", ");
            sb.Append(date.Day);

            if (date.Date != date)
            {
                sb.Append(", ");
                sb.Append(date.Hour);
                sb.Append(", ");
                sb.Append(date.Minute);
                sb.Append(", ");
                sb.Append(date.Second);

                if (date.Millisecond != 0)
                {
                    sb.Append(", ");
                    sb.Append(date.Millisecond);
                }
            }

            sb.Append(")");

            return true;
        }
    }
}
