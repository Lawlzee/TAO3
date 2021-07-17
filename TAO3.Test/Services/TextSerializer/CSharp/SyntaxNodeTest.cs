using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.CSharp;
using TAO3.TextSerializer.CSharp;

namespace TAO3.Test.Services.TextSerializer.CSharp
{
    [TestClass]
    public class SyntaxNodeTest
    {
        private static readonly string _code = @"using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.Internal.Types
{
    internal static class NumberHelper
    {
        public static string? GetNumberSuffix(Type numberType)
        {
            if (_prefixes.ContainsKey(numberType))
            {
                return _prefixes[numberType];
            }
            return null;
        }
    }
}";

        [TestMethod]
        public void SerializeSyntaxNodeTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_code);
            CSharpSyntaxNode compilation = tree.GetCompilationUnitRoot();
            
            string got = new CSharpObjectSerializer().Serialize(compilation);
            Assert.AreEqual(_code, got);
        }

        [TestMethod]
        public void SerializeICSharpNodeTest()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(_code);
            CompilationUnitSyntax compilation = tree.GetCompilationUnitRoot();
            CSharpCompilationUnit cSharpCompilationUnit = new CSharpCompilationUnit(compilation);

            string got = new CSharpObjectSerializer().Serialize(cSharpCompilationUnit);
            Assert.AreEqual(_code, got);
        }
    }
}
