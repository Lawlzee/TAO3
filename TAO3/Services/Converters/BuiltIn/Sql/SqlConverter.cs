using System.CommandLine;
using System.Reactive;
using TAO3.Internal.Types;
using TAO3.TypeProvider;

namespace TAO3.Converters.Sql;

public record SqlConverterParameters
{
    public string? Type { get; init; }
}

public record SqlConverterInputParameters : SqlConverterParameters
{
    public string? TableName { get; init; }
}

public class SqlConverter : IConverterTypeProvider<SqlConverterSettings, SqlConverterParameters>,
    IOutputConfigurableConverter<SqlConverterSettings, SqlConverterInputParameters>
{
    private readonly ITypeProvider<string> _typeProvider;
    private readonly SqlDeserializer _deserializer;
    private readonly ISqlObjectSerializer _serializer;

    public string Format => "sql";
    public IReadOnlyList<string> Aliases => Array.Empty<string>();
    public string MimeType => "text/x-sql";
    public IReadOnlyList<string> FileExtensions => new[] { ".sql" };
    public Dictionary<string, object> Properties { get; }
    public IDomCompiler DomCompiler => _typeProvider;

    public SqlConverter(
        ITypeProvider<string> typeProvider,
        SqlDeserializer deserializer,
        ISqlObjectSerializer serializer)
    {
        _typeProvider = typeProvider;
        _deserializer = deserializer;
        _serializer = serializer;
        Properties = new Dictionary<string, object>();
    }

    public string Serialize(object? value, SqlConverterSettings? settings = null)
    {
        return _serializer.Serialize(value, settings ?? new SqlConverterSettings(null));
    }

    T IConverterTypeProvider<SqlConverterSettings, SqlConverterParameters>.Deserialize<T>(string text, SqlConverterSettings? settings)
    {
        return (T)TypeInferer.Invoke<object>(
            typeof(T),
            typeof(List<>),
            () => Deserialize<Unit>(text, settings));
    }

    public List<T> Deserialize<T>(string text, SqlConverterSettings? settings = null)
        where T : new()
    {
        return _deserializer.Deserialize<T>(text);
    }

    void IInputConfigurableConverter<SqlConverterSettings, SqlConverterParameters>.Configure(Command command)
    {
        command.Add(new Option<string>(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
    }

    void IOutputConfigurableConverter<SqlConverterSettings, SqlConverterInputParameters>.Configure(Command command)
    {
        command.Add(new Option<string>(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
        command.Add(new Option<string>(new[] { "-tn", "--tableName" }, "The name of the table used for the insert commands"));
    }

    SqlConverterSettings IInputConfigurableConverter<SqlConverterSettings, SqlConverterParameters>.GetDefaultSettings()
    {
        return new SqlConverterSettings(null);
    }

    SqlConverterSettings IInputConfigurableConverter<SqlConverterSettings, SqlConverterParameters>.BindParameters(SqlConverterSettings settings, SqlConverterParameters args)
    {
        return settings;
    }

    SqlConverterSettings IOutputConfigurableConverter<SqlConverterSettings, SqlConverterInputParameters>.GetDefaultSettings()
    {
        return new SqlConverterSettings(null);
    }

    SqlConverterSettings IOutputConfigurableConverter<SqlConverterSettings, SqlConverterInputParameters>.BindParameters(SqlConverterSettings settings, SqlConverterInputParameters args)
    {
        return settings with
        {
            TableName = args.TableName
        };
    }

    async Task<IDomType> IConverterTypeProvider<SqlConverterSettings, SqlConverterParameters>.ProvideTypeAsync(IConverterContext<SqlConverterSettings> context, SqlConverterParameters args)
    {
        if (args.Type != null)
        {
            return new DomCollection(new List<IDomType>
            {
                new DomClassReference(args.Type)
            });
        }

        string sql = await context.GetTextAsync();
        return _typeProvider.DomParser.Parse(sql);
    }
}
