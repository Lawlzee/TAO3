using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.Text
{
    public class TextConverter : IConverter<Unit>
    {
        public string Format => "text";
        public IReadOnlyList<string> Aliases => new[] { "Text", "string", "String" };
        public string MimeType => "text/plain";
        public string DefaultType => "string";
        public Dictionary<string, object> Properties { get; }

        public TextConverter()
        {
            Properties = new Dictionary<string, object>();
        }

        object? IConverter<Unit>.Deserialize<T>(string text, Unit unit) => Deserialize<T>(text);
        public object? Deserialize<T>(string text)
        {
            return text;
        }

        string IConverter<Unit>.Serialize(object? value, Unit unit) => Serialize(value);
        public string Serialize(object? value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
