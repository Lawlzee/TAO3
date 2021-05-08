using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace TAO3.Converters
{
    public class LineConverter : IConverter
    {
        public string Format => "line";

        public string DefaultType => "string[]";

        public object? Deserialize<T>(string text, object? settings)
        {
            return Regex.Split(text, @"\r\n|\r|\n");
        }

        public string Serialize(object? value, object? settings)
        {
            if (value is IEnumerable enumerable)
            {
                return string.Join(Environment.NewLine, enumerable);
            }
            return value?.ToString() ?? "";
        }
    }
}
