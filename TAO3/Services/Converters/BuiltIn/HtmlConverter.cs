using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.Html
{
    public class HtmlConverter : IConverter<Unit>
    {
        public string Format => "html";
        public string MimeType => "text/html";
        public IReadOnlyList<string> Aliases => Array.Empty<string>();
        public string DefaultType => "Microsoft.AspNetCore.Html.HtmlString";

        public Dictionary<string, object> Properties { get; }

        public HtmlConverter()
        {
            Properties = new Dictionary<string, object>();
        }

        object? IConverter<Unit>.Deserialize<T>(string text, Unit unit) => Deserialize<T>(text);
        public object? Deserialize<T>(string text)
        {
            return new HtmlString(text);
        }

        string IConverter<Unit>.Serialize(object? value, Unit unit) => Serialize(value);
        public string Serialize(object? value)
        {
            return Formatter.ToDisplayString(value, "text/html");
        }
    }
}
