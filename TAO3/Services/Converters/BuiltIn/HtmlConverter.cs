﻿using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public class HtmlConverter : IConverter
    {
        public string Format => "html";
        public string DefaultType => "Microsoft.AspNetCore.Html.HtmlString";

        public object? Deserialize<T>(string text, object? settings = null)
        {
            return new HtmlString(text);
        }

        public string Serialize(object? value, object? settings = null)
        {
            return Formatter.ToDisplayString(value, "text/html");
        }
    }
}