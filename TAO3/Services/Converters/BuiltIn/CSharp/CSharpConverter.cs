using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.CSharp;

namespace TAO3.Converters.CSharp
{
    public class CSharpConverter : IConverter<Unit>
    {
        public ICSharpObjectSerializer Serializer { get; }

        public string Format => "csharp";
        public IReadOnlyList<string> Aliases => new[] { "CSharp", "c#", "C#" };
        public string DefaultType => typeof(CSharpCompilationUnit).FullName!;

        public CSharpConverter(ICSharpObjectSerializer serializer)
        {
            Serializer = serializer;
        }

        object? IConverter<Unit>.Deserialize<T>(string text, Unit unit) => Deserialize<T>(text);
        public object? Deserialize<T>(string text)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
            CompilationUnitSyntax compilation = tree.GetCompilationUnitRoot();
            return new CSharpCompilationUnit(compilation);
        }

        string IConverter<Unit>.Serialize(object? value, Unit unit) => Serialize(value);
        public string Serialize(object? value)
        {
            return Serializer.Serialize(value);
        }
    }
}
