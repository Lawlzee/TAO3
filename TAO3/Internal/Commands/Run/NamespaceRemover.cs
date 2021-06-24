using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Commands.Run
{
    internal static class NamespaceRemover
    {
        public static string RemoveNamespaces(string code)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var rangesToRemove = root
                .ChildNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .SelectMany(x => new TextSpan[]
                {
                    x.NamespaceKeyword.Span,
                    x.Name.Span,
                    x.OpenBraceToken.Span,
                    x.CloseBraceToken.Span
                })
                .Reverse()
                .ToList();

            StringBuilder sb = new StringBuilder(code);

            foreach (var range in rangesToRemove)
            {
                sb.Remove(range.Start, range.Length);
            }

            return sb.ToString();
        }
    }
}
