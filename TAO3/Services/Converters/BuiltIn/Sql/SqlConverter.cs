using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.Sql;
using TAO3.TypeProvider;

namespace TAO3.Converters.Sql
{
    public record SqlConverterParameters
    {
        public string? Type { get; init; }
    }

    public class SqlConverter : 
        IConverter<Unit>,
        IHandleInputCommand<Unit, SqlConverterParameters>
    {
        private readonly ITypeProvider<string> _typeProvider;
        private readonly SqlDeserializer _deserializer;
        private readonly ISqlObjectSerializer _serializer;

        public string Format => "sql";
        public IReadOnlyList<string> Aliases => Array.Empty<string>();
        public string MimeType => "text/x-sql";
        public string DefaultType => "dynamic";
        public Dictionary<string, object> Properties { get; }

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

        string IConverter<Unit>.Serialize(object? value, Unit unit) => Serialize(value);
        public string Serialize(object? value)
        {
            return _serializer.Serialize(value);
        }

        object? IConverter<Unit>.Deserialize<T>(string text, Unit unit) => Deserialize<T>(text);

        public object? Deserialize<T>(string text)
        {
            return ((dynamic)_deserializer).Deserialize<T>(text);
        }

        public void Configure(Command command)
        {
            command.Add(new Option<string>(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
        }

        public Unit GetDefaultSettings() => Unit.Default;

        public Unit BindParameters(Unit settings, SqlConverterParameters args)
        {
            return Unit.Default;
        }

        public async Task HandleCommandAsync(IConverterContext<Unit> context, SqlConverterParameters args)
        {
            string sql = await context.GetTextAsync();
            string textVariableName = await context.CreatePrivateVariableAsync(sql, typeof(string));

            if (args.Type == null)
            {
                SchemaSerialization schema = _typeProvider.ProvideTypes(sql);

                await context.SubmitCodeAsync($@"{schema.Code}

{schema.RootType} {context.VariableName} = TAO3.Prelude.FromSql<{schema.ElementType}>({textVariableName});");
            }
            else
            {
                await context.SubmitCodeAsync($@"{args.Type} {context.VariableName} = TAO3.Prelude.FromSql<{args.Type}>({textVariableName});");
            }

        }
    }
}
