using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TextSerializer.SQL
{
    internal record SqlColumn(
        string Name,
        Type Type,
        MemberInfo Member);

    internal static class SqlColumnInferer
    {
        //todo: support has one relationship (create fk column)
        public static List<SqlColumn> InferColumns(Type type)
        {
            //todo: complete list/make configurable
            Type[] supportedColumnsTypes =
            {
                typeof(byte),
                typeof(byte?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(short),
                typeof(short?),
                typeof(int),
                typeof(int?),
                typeof(long),
                typeof(long?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(ushort),
                typeof(ushort?),
                typeof(uint),
                typeof(uint?),
                typeof(ulong),
                typeof(ulong?),
                typeof(float),
                typeof(float?),
                typeof(bool),
                typeof(bool?),
                typeof(char),
                typeof(char?),
                typeof(string),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(Guid),
                typeof(Guid?)
            };

            return type
                .GetMembers()
                .Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property)
                .Where(x => !(x is PropertyInfo prop) || (prop.CanRead && prop.GetIndexParameters().Length == 0))
                .Select(x => new SqlColumn(
                    x.Name,
                    x is PropertyInfo prop
                        ? prop.PropertyType
                        : ((FieldInfo)x).FieldType,
                    x))
                .Where(x => x.Type.IsEnum || supportedColumnsTypes.Any(type => x.Type.IsAssignableTo(type)))
                .ToList();
        }
    }
}
