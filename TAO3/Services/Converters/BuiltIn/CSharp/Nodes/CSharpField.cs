using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public class CSharpField : ICSharpMember
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public FieldDeclarationSyntax Syntax { get; }
    public IReadOnlyList<CSharpAttribute> Attributes { get; }
    public CSharpModifiers Modifiers { get; }
    public string Name { get; }
    public CSharpType Type { get; }
    public string? DefaultValue { get; }

    MemberDeclarationSyntax ICSharpMember.Syntax => Syntax;

    public CSharpField(FieldDeclarationSyntax syntax)
    {
        Syntax = syntax;
        Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
        Modifiers = CSharpModifiersHelper.Parse(syntax.Modifiers);
        Type = new CSharpType(syntax.Declaration.Type);
        Name = syntax.Declaration.Variables[0].Identifier.ToString();
        DefaultValue = syntax.Declaration.Variables[0].Initializer?.Value?.ToString();
    }
}
