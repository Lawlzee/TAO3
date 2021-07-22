using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp
{
    internal class CollectionInitializerTypeConverter<T> : TypeConverter<IEnumerable<T>>
    {
        public override bool Convert(StringBuilder sb, IEnumerable<T> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            Type type = obj.GetType();

            int matches = type.GetMethods()
                .Where(x => x.Name == "Add")
                .Where(x => !x.IsStatic)
                .Where(x => x.GetParameters().Length == 1)
                .Where(x => x.GetParameters()[0].ParameterType == typeof(T))
                .Where(x => x.IsPublic || x.IsAssembly)
                .Count();

            if (matches != 1 && !type.IsArray)
            {
                return false;
            }

            bool isDictionary = type
                .GetParentTypes()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (isDictionary)
            {
                return false;
            }

            List<T> values = obj.ToList();

            sb.Append("new ");

            if (values.Count == 0)
            {
                if (type.IsArray)
                {
                    //Ugly, but it kinds of works
                    sb.Append(obj.GetType().PrettyPrint()
                        .Replace("[", "[0")
                        .Replace(",", ", 0"));
                }
                else
                {
                    sb.Append(obj.GetType().PrettyPrint());
                    sb.Append("()");
                }

                return true;
            }

            sb.AppendLine(obj.GetType().PrettyPrint());
            sb.Append(options.Indentation);
            sb.AppendLine("{");

            ObjectSerializerOptions elementOptions = options.Indent();

            bool isFirst = true;
            foreach (T element in obj)
            {
                if (!isFirst)
                {
                    sb.AppendLine(",");
                }

                sb.Append(elementOptions.Indentation);
                serializer.Serialize(sb, element, elementOptions);

                isFirst = false;
            }

            sb.AppendLine();
            sb.Append(options.Indentation);
            sb.Append("}");

            return true;
        }
    }
}
