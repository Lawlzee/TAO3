using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.Text
{
    public class TextConverter : IConverter
    {
        public string Format => "text";
        public string DefaultType => "string";

        public object? Deserialize<T>(string text)
        {
            return text;
        }

        public string Serialize(object? value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
