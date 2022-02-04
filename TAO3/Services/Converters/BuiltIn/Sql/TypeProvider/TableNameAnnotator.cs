using TAO3.TypeProvider;

namespace TAO3.Converters.Sql;

public class TableNameAnnotator : IClassAnnotator
{
    public void Annotate(ClassSchema clazz, ClassAnnotatorContext context)
    {
        context.Using(typeof(TableNameAttribute).Namespace!);
        context.StringBuilder.Append(@$"[TableName(""{clazz.FullName.Replace("\"", "\"\"")}"")]
");
    }
}
