using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Types;

namespace TAO3.TextSerializer.SQL
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
            sb.Append(type.PrettyPrint());
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

        private record Member(string Name, object? Value);

        //todo: cache members
        //todo: support has one relationship (create fk column)
        private List<Member> GetMembers(object obj)
        {
            //todo: complete list/make configurable
            Type[] supportedColumnsTypes =
            {
                typeof(byte),
                typeof(decimal),
                typeof(double),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(sbyte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(float),
                typeof(bool),
                typeof(char),
                typeof(string),
                typeof(DateTime),
                typeof(Guid),
                typeof(Enum)
            };

            return obj.GetType()
                .GetMembers()
                .Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property)
                .Where(x => !(x is PropertyInfo prop) || (prop.CanRead && prop.GetIndexParameters().Length == 0))
                .Select(x => new
                {
                    x.Name,
                    Type = x is PropertyInfo prop
                        ? prop.PropertyType
                        : ((FieldInfo)x).FieldType,
                    Value = x is PropertyInfo prop2
                        ? prop2.GetValue(obj, null)
                        : ((FieldInfo)x).GetValue(obj)
                })
                .Where(x => supportedColumnsTypes.Any(type => x.Type.IsAssignableTo(type)))
                .Select(x => new Member(
                    x.Name,
                    x.Value))
                .ToList();


        }
    }
}
