using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp
{
    internal class AnonymousTypeConverter : TypeConverter<object>
    {
        public override bool Convert(StringBuilder sb, object obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            if (!obj.GetType().IsAnonymous())
            {
                return false;
            }

            sb.AppendLine("new");
            sb.Append(options.Indentation);
            sb.Append("{");

            ObjectSerializerOptions propOptions = options.Indent();

            PropertyInfo[] properties = obj.GetType().GetProperties();

            int i = 0;
            foreach (PropertyInfo propertyInfo in properties)
            {
                sb.AppendLine();
                sb.Append(propOptions.Indentation);

                sb.Append(propertyInfo.Name);
                sb.Append(" = ");

                serializer.Serialize(sb, propertyInfo.GetValue(obj), propOptions);

                i++;
                if (i != properties.Length)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine();
            sb.Append(options.Indentation);
            sb.Append("}");

            return true;
        }

        public override void Dispose()
        {

        }
    }
}
