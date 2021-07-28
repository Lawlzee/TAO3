using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.Sql
{
    internal class ObjectTypeConverter : TypeConverter<object>
    {
        public override bool Convert(StringBuilder sb, object obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            Type type = obj.GetType();

            List<Member> members = GetMembers(obj);
            
            if (members.Count == 0)
            {
                return false;
            }

            sb.Append("INSERT INTO [");
            sb.Append(GetTableName(type));
            sb.Append("] ([");

            for (int i = 0; i < members.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append("], [");
                }
                sb.Append(members[i].Name);
            }

            sb.Append("]) VALUES(");

            for (int i = 0; i < members.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                serializer.Serialize(sb, members[i].Value, options);
            }

            sb.Append(");");

            return true;
        }

        private string GetTableName(Type type)
        {
            return type.GetCustomAttribute<TableNameAttribute>()?.Name ?? type.PrettyPrint();
        }

        private record Member(string Name, object? Value);

        //todo: cache infered SqlColumns
        private List<Member> GetMembers(object obj)
        {
            return SqlColumnInferer.InferColumns(obj.GetType())
                .Select(x => new Member(
                    x.Name,
                    x.Member is PropertyInfo prop
                        ? prop.GetValue(obj, null)
                        : ((FieldInfo)x.Member).GetValue(obj)))
                .ToList();


        }
    }
}
