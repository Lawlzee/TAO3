using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.CSharp
{
    public class CSharpMethod : ICSharpMember
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public MethodDeclarationSyntax Syntax { get; }
        public IReadOnlyList<CSharpAttribute> Attributes { get; }
        public CSharpModifiers Modifiers { get; }
        public CSharpType ReturnType { get; }
        public string Name { get; }
        public IReadOnlyList<CSharpParameter> Parameters { get; }
        public string? Implementation { get; }

        MemberDeclarationSyntax ICSharpMember.Syntax => Syntax;

        public CSharpMethod(MethodDeclarationSyntax syntax)
        {
            Syntax = syntax;
            Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
            Modifiers = CSharpModifiersHelper.Parse(syntax.Modifiers);
            ReturnType = new CSharpType(syntax.ReturnType);
            Name = syntax.Identifier.ToString();
            Parameters = syntax.ParameterList
                .Parameters
                .Select(x => new CSharpParameter(x))
                .ToList();
            Implementation = syntax.ExpressionBody?.Expression?.ToString()
                ?? syntax.Body?.ToString();
        }
    }
}
