using System.Reflection;
using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class ObjectTypeConverter : TypeConverter<object, SqlConverterSettings>
{
    public override bool Convert(object obj, ObjectSerializerContext<SqlConverterSettings> context)
    {
        Type type = obj.GetType();

        List<Member> members = GetMembers(obj);
        
        if (members.Count == 0)
        {
            return false;
        }

        context.Append("INSERT INTO [");
        context.Append(GetTableName(type, context.Settings));
        context.Append("] ([");

        for (int i = 0; i < members.Count; i++)
        {
            if (i > 0)
            {
                context.Append("], [");
            }
            context.Append(members[i].Name);
        }

        context.Append("]) VALUES(");

        for (int i = 0; i < members.Count; i++)
        {
            if (i > 0)
            {
                context.Append(", ");
            }
            context.Serialize(members[i].Value);
        }

        context.Append(");");

        return true;
    }

    private string GetTableName(Type type, SqlConverterSettings settings)
    {
        return settings.TableName 
            ?? type.GetCustomAttribute<TableNameAttribute>()?.Name 
            ?? type.PrettyPrint();
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
