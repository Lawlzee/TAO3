using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class DateTimeTypeConverter : TypeConverter<DateTime, SqlConverterSettings>
{
    public override bool Convert(DateTime date, ObjectSerializerContext<SqlConverterSettings> context)
    {
        context.Append("'");

        context.Append(date.Year);
        context.Append("-");
        context.Append(date.Month.ToString("00"));
        context.Append("-");
        context.Append(date.Day.ToString("00"));

        if (date.Date != date)
        {
            context.Append(" ");
            context.Append(date.Hour.ToString("00"));
            context.Append(":");
            context.Append(date.Minute.ToString("00"));
            context.Append(":");
            context.Append(date.Second.ToString("00"));

            if (date.Millisecond != 0)
            {
                context.Append(".");
                context.Append(date.Millisecond);
            }
        }

        context.Append("'");

        return true;
    }
}
