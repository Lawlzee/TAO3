using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.Html
{
    public class HtmlConverter : IConverter
    {
        public string Format => "html";
        public string DefaultType => "Microsoft.AspNetCore.Html.HtmlString";

        public IReadOnlyList<string> Aliases => new[] { "HTML" };

        public object? Deserialize<T>(string text)
        {
            return new HtmlString(text);
        }

        public string Serialize(object? value)
        {
            return Formatter.ToDisplayString(value, "text/html");
        }
    }
}
