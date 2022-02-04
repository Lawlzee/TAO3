using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TAO3.Converters.CSharp;

public class CSharpConverter : IConverter<CSharpCompilationUnit>
{
    public ICSharpObjectSerializer Serializer { get; }

    public string Format => "csharp";
    public IReadOnlyList<string> Aliases => new[] { "c#" };
    public string MimeType => "text/x-csharp";
    public IReadOnlyList<string> FileExtensions => new[] { ".cs" };
    public Dictionary<string, object> Properties { get; }

    public CSharpConverter(ICSharpObjectSerializer serializer)
    {
        Serializer = serializer;
        Properties = new Dictionary<string, object>();
    }

    public CSharpCompilationUnit Deserialize(string text)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
        CompilationUnitSyntax compilation = tree.GetCompilationUnitRoot();
        return new CSharpCompilationUnit(compilation);
    }

    public string Serialize(object? value)
    {
        return Serializer.Serialize(value, new CSharpSerializerSettings());
    }
}
