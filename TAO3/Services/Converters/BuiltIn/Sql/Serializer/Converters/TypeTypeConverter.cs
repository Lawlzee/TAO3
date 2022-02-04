using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class TypeTypeConverter : TypeConverter<Type, SqlConverterSettings>
{
    private static readonly Dictionary<Type, string> _typeMappingByType = new Dictionary<Type, string>()
    {
        [typeof(byte)] = "TINYINT NOT NULL",
        [typeof(byte?)] = "TINYINT NULL",
        [typeof(decimal)] = "DECIMAL(18, 2) NOT NULL",
        [typeof(decimal?)] = "DECIMAL(18, 2) NULL",
        [typeof(double)] = "FLOAT NOT NULL",
        [typeof(double?)] = "FLOAT NULL",
        [typeof(short)] = "SMALLINT NOT NULL",
        [typeof(short?)] = "SMALLINT NULL",
        [typeof(int)] = "INT NOT NULL",
        [typeof(int?)] = "INT NULL",
        [typeof(long)] = "BIGINT NOT NULL",
        [typeof(long?)] = "BIGINT NULL",
        [typeof(sbyte)] = "SMALLINT NOT NULL",
        [typeof(sbyte?)] = "SMALLINT NULL",
        [typeof(ushort)] = "INT NOT NULL",
        [typeof(ushort?)] = "INT NULL",
        [typeof(uint)] = "BIGINT NOT NULL",
        [typeof(uint?)] = "BIGINT NULL",
        [typeof(ulong)] = "BIGINT NOT NULL",
        [typeof(ulong?)] = "BIGINT NULL",
        [typeof(float)] = "REAL NOT NULL",
        [typeof(float?)] = "REAL NULL",
        [typeof(bool)] = "BIT NOT NULL",
        [typeof(bool?)] = "BIT NULL",
        [typeof(char)] = "NVARCHAR(1) NOT NULL",
        [typeof(char?)] = "NVARCHAR(1) NULL",
        [typeof(string)] = "nvarchar(MAX) NULL",
        [typeof(DateTime)] = "DATETIME2 NOT NULL",
        [typeof(DateTime?)] = "DATETIME2 NULL",
        [typeof(Guid)] = "UNIQUEIDENTIFIER NOT NULL",
        [typeof(Guid?)] = "UNIQUEIDENTIFIER NULL",
        [typeof(byte[])] = "VARBINARY(MAX)"

    };

    public override bool Convert(Type type, ObjectSerializerContext<SqlConverterSettings> context)
    {
        context.Append("CREATE TABLE [");
        context.Append(type.PrettyPrint());
        context.AppendLine("] (");

        List<SqlColumn> columns = SqlColumnInferer.InferColumns(type);

        if (columns.Count == 0)
        {
            return false;
        }

        ObjectSerializerContext<SqlConverterSettings> columnsContext = context.Indent();

        SqlColumn? idColumn = columns
            .Where(x => x.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();

        if (idColumn == null)
        {
            columnsContext.Append(columnsContext.Indentation);
            columnsContext.AppendLine("[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (NEWID()),");
        }

        for (int i = 0; i < columns.Count; i++)
        {
            SqlColumn column = columns[i];

            if (i > 0)
            {
                columnsContext.AppendLine(",");
            }

            columnsContext.Append(columnsContext.Indentation);
            columnsContext.Append("[");
            columnsContext.Append(column.Name);
            columnsContext.Append("] ");
            columnsContext.Append(GetColumnType(column.Type));


            if (column == idColumn)
            {
                columnsContext.Append(" PRIMARY KEY");
                if (column.Type == typeof(Guid) || column.Type == typeof(Guid?))
                {

                    columnsContext.Append(" DEFAULT (NEWID())");
                }
                else if (column.Type == typeof(int) || column.Type == typeof(int?) || column.Type == typeof(long) || column.Type == typeof(long?))
                {
                    columnsContext.Append(" IDENTITY(1,1)");
                }
            }
        }

        columnsContext.Append(");");

        return true;
    }

    private string GetColumnType(Type clrType)
    {
        Type type = clrType;
        if (clrType.IsEnum)
        {
            type = clrType.GetEnumUnderlyingType();
        }

        return _typeMappingByType.GetValueOrDefault(type) ?? throw new ArgumentException($"Invalid column type '{clrType.FullName}'");
    }
}
