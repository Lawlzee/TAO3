using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TAO3.Internal.Types;

namespace TAO3.TextSerializer.CSharp
{
    internal class CollectionInitializerTypeConverter<T> : TypeConverter<IEnumerable<T>>
    {
        public override bool Convert(StringBuilder sb, IEnumerable<T> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            int matches = obj.GetType().GetMethods()
                .Where(x => x.Name == "Add")
                .Where(x => !x.IsStatic)
                .Where(x => x.GetParameters().Length == 1)
                .Where(x => x.GetParameters()[0].ParameterType == typeof(T))
                .Where(x => x.IsPublic || x.IsAssembly)
                .Count();

            if (matches != 1 && !obj.GetType().IsArray)
            {
                return false;
            }

            bool isDictionary = obj.GetType()
                .GetParentTypes()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (isDictionary)
            {
                return false;
            }

            sb.Append("new ");
            sb.AppendLine(obj.GetType().PrettyPrint());
            sb.Append(options.Indentation);
            sb.AppendLine("{");

            ObjectSerializerOptions elementOptions = options.Indent();

            foreach (T element in obj)
            {
                sb.Append(elementOptions.Indentation);
                serializer.Serialize(sb, element, elementOptions);
                sb.AppendLine(",");
            }

            sb.Append(options.Indentation);
            sb.Append("}");

            return true;
        }
    }
}
