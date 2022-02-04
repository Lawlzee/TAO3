namespace TAO3.Converters.Sql;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TableNameAttribute : Attribute
{
    public string Name { get; }

    public TableNameAttribute(string name)
    {
        Name = name;
    }
}
