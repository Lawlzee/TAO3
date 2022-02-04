using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public class CSharpConstructor : ICSharpMember
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ConstructorDeclarationSyntax Syntax { get; }
    public IReadOnlyList<CSharpAttribute> Attributes { get; }
    public CSharpModifiers Modifiers { get; }
    public string Name { get; }
    public IReadOnlyList<CSharpParameter> Parameters { get; }
    public string? Implementation { get; }

    MemberDeclarationSyntax ICSharpMember.Syntax => Syntax;

    public CSharpConstructor(ConstructorDeclarationSyntax syntax)
    {
        Syntax = syntax;
        Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
        Modifiers = CSharpModifiersHelper.Parse(syntax.Modifiers);
        Name = syntax.Identifier.ToString();
        Parameters = syntax.ParameterList
            .Parameters
            .Select(x => new CSharpParameter(x))
            .ToList();
        Implementation = syntax.ExpressionBody?.Expression?.ToString()
            ?? syntax.Body?.ToString();
    }
}
