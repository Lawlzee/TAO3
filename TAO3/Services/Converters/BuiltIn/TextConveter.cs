using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public class TextConveter : IConverter
    {
        public string Format => "text";
        public string DefaultType => "string";

        public object? Deserialize<T>(string text, object? settings = null)
        {
            return text;
        }

        public string Serialize(object? value, object? settings = null)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
