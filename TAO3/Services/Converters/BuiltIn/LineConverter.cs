using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TAO3.Converters.Line
{
    public class LineConverter : IConverter<Unit>
    {
        public string Format => "line";
        public IReadOnlyList<string> Aliases => Array.Empty<string>();
        public string DefaultType => "List<string>";
        public string MimeType => "text/plain";
        public Dictionary<string, object> Properties { get; }

        public LineConverter()
        {
            Properties = new Dictionary<string, object>();
        }

        object? IConverter<Unit>.Deserialize<T>(string text, Unit unit) => Deserialize<T>(text);

        public object? Deserialize<T>(string text)
        {
            return Regex.Split(text, @"\r\n|\r|\n").ToList();
        }

        string IConverter<Unit>.Serialize(object? value, Unit unit) => Serialize(value);

        public string Serialize(object? value)
        {
            if (value is string str)
            {
                return str;
            }

            if (value is IEnumerable enumerable)
            {
                return string.Join(Environment.NewLine, enumerable.Cast<object>());
            }
            return value?.ToString() ?? "";
        }
    }
}
