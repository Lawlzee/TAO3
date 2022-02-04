using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public abstract class CSharpBaseTypeDeclaration : ICSharpTypeDeclaration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TypeDeclarationSyntax Syntax { get; }
    public IReadOnlyList<CSharpAttribute> Attributes { get; }
    public CSharpModifiers Modifiers { get; }
    public string Name { get; }
    public string Identifier { get; }
    public IReadOnlyList<CSharpTypeParameter> TypeParameters { get; }
    public IReadOnlyList<CSharpType> Parents { get; }
    public IReadOnlyList<ICSharpMember> Members { get; }

    BaseTypeDeclarationSyntax ICSharpTypeDeclaration.Syntax => Syntax;
    MemberDeclarationSyntax ICSharpMember.Syntax => Syntax;

    public IReadOnlyList<CSharpField> Fields { get; }
    public IReadOnlyList<CSharpProperty> Properties { get; }
    public IReadOnlyList<CSharpConstructor> Constructors { get; }
    public IReadOnlyList<CSharpMethod> Methods { get; }

    protected CSharpBaseTypeDeclaration(TypeDeclarationSyntax syntax)
    {
        Syntax = syntax;
        Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
        Modifiers = CSharpModifiersHelper.Parse(syntax.Modifiers);
        Identifier = syntax.Identifier.ToString();
        Name = syntax.Identifier.ToString() + syntax.TypeParameterList?.ToString();
        Parents = syntax.BaseList == null
            ? new List<CSharpType>()
            : syntax.BaseList
                .Types
                .Select(x => new CSharpType(x.Type))
                .ToList();

        TypeParameters = syntax.TypeParameterList == null
            ? new List<CSharpTypeParameter>()
            : syntax.TypeParameterList
                .Parameters
                .Select(x => new CSharpTypeParameter(x))
                .ToList();

        Members = syntax.Members
            .SelectMany(x => ICSharpMember.Create(x))
            .ToList();

        Fields = Members.OfType<CSharpField>().ToList();
        Properties = Members.OfType<CSharpProperty>().ToList();
        Constructors = Members.OfType<CSharpConstructor>().ToList();
        Methods = Members.OfType<CSharpMethod>().ToList();
    }
}

public class CSharpTypeParameter : ICSharpNode
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TypeParameterSyntax Syntax { get; }
    CSharpSyntaxNode ICSharpNode.Syntax => Syntax;

    public string Name { get; }
    public bool IsIn { get; }
    public bool IsOut { get; }

    public CSharpTypeParameter(TypeParameterSyntax syntax)
    {
        Syntax = syntax;
        Name = syntax.Identifier.Text;
        IsIn = syntax.VarianceKeyword.Text == "in";
        IsOut = syntax.VarianceKeyword.Text == "out";
    }
}
