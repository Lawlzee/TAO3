using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Formatting
{
    public class CSharpFormatter : IFormatter
    {
        public string Format(string text)
        {
            return CSharpSyntaxTree
                .ParseText(text)
                .GetRoot()
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
