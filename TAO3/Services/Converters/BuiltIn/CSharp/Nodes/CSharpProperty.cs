using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public class CSharpProperty : ICSharpMember
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public PropertyDeclarationSyntax Syntax { get; }
    public IReadOnlyList<CSharpAttribute> Attributes { get; }
    public CSharpModifiers Modifiers { get; }
    public CSharpType Type { get; }
    public string Name { get; }

    public CSharpPropertyArrowGetter? ArrowGetter { get; }
    public CSharpPropertyAccessor? Getter { get; }
    public CSharpPropertyAccessor? Setter { get; }

    public string? DefaultValue { get; }

    MemberDeclarationSyntax ICSharpMember.Syntax => Syntax;

    public CSharpProperty(PropertyDeclarationSyntax syntax)
    {
        Syntax = syntax;
        Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
        Modifiers = CSharpModifiersHelper.Parse(syntax.Modifiers);
        Type = new CSharpType(syntax.Type);
        Name = syntax.Identifier.ToString();
        DefaultValue = syntax.Initializer?.Value?.ToString();

        if (syntax.ExpressionBody != null)
        {
            ArrowGetter = new CSharpPropertyArrowGetter(syntax.ExpressionBody);
            return;
        }

        if (syntax.AccessorList == null)
        {
            return;
        }

        List<CSharpPropertyAccessor> accessors = syntax.AccessorList
            .Accessors
            .Select(x => new CSharpPropertyAccessor(x))
            .ToList();

        foreach (var accessor in accessors)
        {
            if (accessor.Syntax.Keyword.Text == "get")
            {
                Getter = accessor;
            }
            else if (accessor.Syntax.Keyword.Text == "set")
            {
                Setter = accessor;
            }
        }
    }
}

public class CSharpPropertyArrowGetter : ICSharpNode
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ArrowExpressionClauseSyntax Syntax { get; }
    CSharpSyntaxNode ICSharpNode.Syntax => Syntax;

    public string Expression { get; }

    public CSharpPropertyArrowGetter(ArrowExpressionClauseSyntax syntax)
    {
        Syntax = syntax;
        Expression = syntax.Expression.ToString();
    }
}

public class CSharpPropertyAccessor : ICSharpNode
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public AccessorDeclarationSyntax Syntax { get; }
    CSharpSyntaxNode ICSharpNode.Syntax => Syntax;

    public IReadOnlyList<CSharpAttribute> Attributes { get; }
    public CSharpModifiers Modifiers { get; }
    public string? Expression { get; }

    public CSharpPropertyAccessor(AccessorDeclarationSyntax syntax)
    {
        Syntax = syntax;
        Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
        Modifiers = CSharpModifiersHelper.Parse(syntax.Modifiers);
        Expression = syntax.ExpressionBody?.Expression.ToString()
            ?? syntax.Body?.ToString();
    }
}
