using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.CSharp;
using TAO3.TextSerializer;
using TAO3.TextSerializer.CSharp;

namespace TAO3.Converters
{
    public class CSharpConverter : IConverter
    {
        private readonly ICSharpObjectSerializer _serializer;

        public string Format => "C#";
        public string DefaultType => typeof(CSharpCompilationUnit).FullName!;

        public CSharpConverter(ICSharpObjectSerializer initializerGenerator)
        {
            _serializer = initializerGenerator;
        }

        public object? Deserialize<T>(string text, object? settings = null)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
            CompilationUnitSyntax compilation = tree.GetCompilationUnitRoot();
            return new CSharpCompilationUnit(compilation);
        }

        public string Serialize(object? value, object? settings = null)
        {
            return _serializer.Serialize(value);
        }
    }
}
