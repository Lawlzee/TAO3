using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive.Formatting;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TAO3.TypeProvider;

namespace TAO3.Converters.Html
{
    public class HtmlConverter : IConverterTypeProvider
    {
        public string Format => "html";
        public string MimeType => "text/html";
        public IReadOnlyList<string> Aliases => Array.Empty<string>();

        public Dictionary<string, object> Properties { get; }
        public IDomCompiler DomCompiler { get; } 

        public HtmlConverter()
        {
            Properties = new Dictionary<string, object>();
            DomCompiler = new DomCompiler(Format, IDomSchematizer.Default, IDomSchemaSerializer.Default);
        }

        T IConverterTypeProvider<Unit>.Deserialize<T>(string text) => (T)(object)Deserialize(text);
        public HtmlString Deserialize(string text)
        {
            return new HtmlString(text);
        }

        public string Serialize(object? value)
        {
            return Formatter.ToDisplayString(value, "text/html");
        }

        public Task<IDomType> ProvideTypeAsync(IConverterContext context)
        {
            return Task.FromResult<IDomType>(new DomClassReference(typeof(HtmlString)));
        }
    }
}
