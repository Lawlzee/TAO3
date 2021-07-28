﻿using System;
using System.Collections.Generic;
using System.Text;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp
{
    internal class DateTimeTypeConverter : TypeConverter<DateTime>
    {
        public override bool Convert(StringBuilder sb, DateTime date, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append("new DateTime(");
            sb.Append(date.Year);
            sb.Append(", ");
            sb.Append(date.Month.ToString("00"));
            sb.Append(", ");
            sb.Append(date.Day.ToString("00"));

            if (date.Date != date)
            {
                sb.Append(", ");
                sb.Append(date.Hour.ToString("00"));
                sb.Append(", ");
                sb.Append(date.Minute.ToString("00"));
                sb.Append(", ");
                sb.Append(date.Second.ToString("00"));

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