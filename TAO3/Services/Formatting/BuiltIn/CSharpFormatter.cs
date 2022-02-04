using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TAO3.Formatting;

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
