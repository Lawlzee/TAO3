using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.Sql;
using TAO3.Internal.Types;
using TAO3.TypeProvider;

namespace TAO3.Converters.Sql
{
    public record SqlConverterParameters
    {
        public string? Type { get; init; }
    }

    public class SqlConverter : IConverterTypeProvider<SqlConverterParameters>
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

        public string Serialize(object? value)
        {
            return _serializer.Serialize(value);
        }

        T IConverterTypeProvider<SqlConverterParameters>.Deserialize<T>(string text)
        {
            return (T)TypeInferer.Invoke<object>(
                typeof(T),
                typeof(List<>),
                () => Deserialize<Unit>(text));
        }

        public List<T> Deserialize<T>(string text)
            where T : new()
        {
            return _deserializer.Deserialize<T>(text);
        }

        public void Configure(Command command)
        {
            command.Add(new Option<string>(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
        }

        public async Task<IDomType> ProvideTypeAsync(IConverterContext context, SqlConverterParameters args)
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
}
