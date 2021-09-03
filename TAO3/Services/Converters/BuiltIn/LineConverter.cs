﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TAO3.Converters.Line
{
    public class LineConverter : IConverter<List<string>>
    {
        public string Format => "line";
        public IReadOnlyList<string> Aliases => Array.Empty<string>();
        public string MimeType => "text/plain";
        public Dictionary<string, object> Properties { get; }

        public LineConverter()
        {
            Properties = new Dictionary<string, object>();
        }

        public List<string> Deserialize(string text)
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
