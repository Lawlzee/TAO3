using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public enum CSharpTypeKind
{
    Other,
    Array,
    GenericType,
    Type,
    Pointer,
    Tuple
}

public class CSharpType
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TypeSyntax Syntax { get; }
    public CSharpTypeKind Kind { get; }

    public string Name { get; }
    public string Identifier { get; }

    public bool IsNullable { get; }
    public bool IsRef { get; }

    public int? Rank { get; }
    public IReadOnlyList<CSharpType>? TypeArguments { get; }
    public IReadOnlyList<CSharpTupleElement>? TupleElements { get; }

    public CSharpType(TypeSyntax syntax)
    {
        Syntax = syntax;
        Name = syntax.ToString();

        while (true)
        {
            if (syntax is AliasQualifiedNameSyntax alias)
            {
                syntax = alias.Name;
            }
            else if (syntax is QualifiedNameSyntax qualified)
            {
                syntax = qualified.Right;
            }
            else if (syntax is NullableTypeSyntax nullable)
            {
                IsNullable = true;
                syntax = nullable.ElementType;
            }
            else if (syntax is RefTypeSyntax refType)
            {
                IsRef = true;
                syntax = refType.Type;
            }
            else
            {
                break;
            }
        }

        switch (syntax)
        {
            case ArrayTypeSyntax x:
                Kind = CSharpTypeKind.Array;
                Rank = x.RankSpecifiers[0].Rank;
                Identifier = x.ElementType.ToString();
                TypeArguments = new[] { new CSharpType(x.ElementType) };
                break;
            case IdentifierNameSyntax x:
                Kind = CSharpTypeKind.Type;
                Identifier = x.ToString();
                break;
            case PredefinedTypeSyntax x:
                Kind = CSharpTypeKind.Type;
                Identifier = x.ToString();
                break;
            case GenericNameSyntax x:
                Kind = CSharpTypeKind.GenericType;
                Identifier = x.Identifier.ToString();
                TypeArguments = x.TypeArgumentList
                    .Arguments
                    .Select(x => new CSharpType(x))
                    .ToList();
                break;
            case PointerTypeSyntax x:
                Kind = CSharpTypeKind.Pointer;
                Identifier = x.ElementType.ToString();
                TypeArguments = new[] { new CSharpType(x.ElementType) };
                break;
            case TupleTypeSyntax x:
                Kind = CSharpTypeKind.Tuple;
                Identifier = $"({new string(',', x.Elements.Count - 1)})";
                TupleElements = x.Elements
                    .Select(y => new CSharpTupleElement(y))
                    .ToList();
                break;
            default:
                Identifier = Name;
                break;
        }
    }
}

public class CSharpTupleElement
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TupleElementSyntax Syntax { get; }
    public string Name { get;}
    public CSharpType Type { get; }

    public CSharpTupleElement(TupleElementSyntax syntax)
    {
        Syntax = syntax;
        Name = syntax.Identifier.Text;
        Type = new CSharpType(syntax.Type);
    }
}
