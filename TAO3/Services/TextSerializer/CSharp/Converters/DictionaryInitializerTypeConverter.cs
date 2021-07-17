using System;
using System.Collections.Generic;
using System.Text;
using TAO3.Internal.Types;

namespace TAO3.TextSerializer.CSharp
{
    internal class DictionaryInitializerTypeConverter<TKey, TValue> : TypeConverter<IDictionary<TKey, TValue>>
    {
        public override bool Convert(StringBuilder sb, IDictionary<TKey, TValue> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            if (obj.GetType().GetConstructor(Type.EmptyTypes) == null)
            {
                return false;
            }

            sb.Append("new ");
            sb.Append(obj.GetType().PrettyPrint());
            sb.Append("()");

            if (obj.Count == 0)
            {
                return true;
            }

            sb.AppendLine();
            sb.Append(options.Indentation);
            sb.AppendLine("{");

            ObjectSerializerOptions elementOptions = options.Indent();

            foreach (KeyValuePair<TKey, TValue> kvp in obj)
            {
                sb.Append(elementOptions.Indentation);
                sb.Append("[");
                serializer.Serialize(sb, kvp.Key, elementOptions);
                sb.Append("] = ");
                serializer.Serialize(sb, kvp.Value, elementOptions);
                sb.AppendLine(",");
            }

            sb.Append(options.Indentation);
            sb.Append("}");

            return true;
        }
    }
}
