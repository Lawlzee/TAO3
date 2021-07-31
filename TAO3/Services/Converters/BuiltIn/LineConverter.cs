using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TAO3.Converters.Line
{
    public class LineConverter : IConverter
    {
        public string Format => "line";

        public string DefaultType => "List<string>";

        public IReadOnlyList<string> Aliases => new[] { "Line" };

        public object? Deserialize<T>(string text)
        {
            return Regex.Split(text, @"\r\n|\r|\n").ToList();
        }

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
