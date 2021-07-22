using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TextSerializer;

namespace TAO3.Converters.SQL
{
    internal class CollectionInitializerTypeConverter<T> : TypeConverter<IEnumerable<T>>
    {
        public override bool Convert(StringBuilder sb, IEnumerable<T> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            bool isFirst = true;
            foreach (T element in obj)
            {
                if (!isFirst)
                {
                    sb.AppendLine();
                }

                sb.Append(options.Indentation);
                serializer.Serialize(sb, element, options);

                isFirst = false;
            }
            return true;
        }
    }
}
