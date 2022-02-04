using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public class CSharpEnum : ICSharpTypeDeclaration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public EnumDeclarationSyntax Syntax { get; }
    public IReadOnlyList<CSharpAttribute> Attributes { get; }
    public CSharpModifiers Modifiers { get; }
    public string Name { get; }
    public IReadOnlyList<CSharpType> Parents { get; }
    public IReadOnlyList<CSharpEnumMember> Members { get; }

    IReadOnlyList<ICSharpMember> ICSharpTypeDeclaration.Members => Members;
    BaseTypeDeclarationSyntax ICSharpTypeDeclaration.Syntax => Syntax;
    MemberDeclarationSyntax ICSharpMember.Syntax => Syntax;

    public CSharpEnum(EnumDeclarationSyntax syntax)
    {
        Syntax = syntax;
        Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
        Modifiers = CSharpModifiersHelper.Parse(syntax.Modifiers);
        Name = syntax.Identifier.ToString();

        Parents = syntax.BaseList == null
            ? new List<CSharpType>()
            : syntax.BaseList
                .Types
                .Select(x => new CSharpType(x.Type))
                .ToList();

        Members = syntax.Members
            .Select(x => new CSharpEnumMember(x))
            .ToList();
    }
}

public class CSharpEnumMember : ICSharpMember
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public EnumMemberDeclarationSyntax Syntax { get; }
    public IReadOnlyList<CSharpAttribute> Attributes { get; }
    public string Name { get; }
    public string? Value { get; }

    MemberDeclarationSyntax ICSharpMember.Syntax => Syntax;
    public CSharpModifiers Modifiers { get; }

    public CSharpEnumMember(EnumMemberDeclarationSyntax syntax)
    {
        Syntax = syntax;
        Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
        Name = syntax.Identifier.ToString();
        Value = syntax.EqualsValue == null 
            ? null
            : syntax.EqualsValue.Value.ToString();

        Modifiers = CSharpModifiersHelper.Parse(syntax.Modifiers);
    }
}
