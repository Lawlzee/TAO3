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

namespace TAO3.Converters
{
    public class CSharpConverter : IConverter
    {
        public string Format => "C#";
        public string DefaultType => typeof(CSharpCompilationUnit).FullName!;

        public object? Deserialize<T>(string text, object? settings = null)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
            CompilationUnitSyntax compilation = tree.GetCompilationUnitRoot();
            return new CSharpCompilationUnit(compilation);
        }

        public string Serialize(object? value, object? settings = null)
        {
            if (value == null)
            {
                return "";
            }

            if (value is SyntaxNode syntaxNode)
            {
                return value.ToString()!;
            }

            if (value.GetType().Namespace == typeof(CSharpCompilationUnit).Namespace)
            {
                return ((dynamic)value).Syntax.ToString();
            }

            bool isEnumerable = value.GetType().GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Where(x => x.GenericTypeArguments[0].IsAssignableTo(typeof(SyntaxNode))
                    || x.GenericTypeArguments[0].Namespace == typeof(CSharpCompilationUnit).Namespace)
                .Any();

            if (isEnumerable)
            {
                return string.Join(
                    Environment.NewLine, 
                    ((IEnumerable)value)
                        .Cast<object>()
                        .Select(x => Serialize(x, settings)));
            }


            throw new NotSupportedException(value.GetType().FullName);
        }
    }
}
