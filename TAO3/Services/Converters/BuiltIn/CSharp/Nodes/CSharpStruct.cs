using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public class CSharpStruct : CSharpBaseTypeDeclaration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new StructDeclarationSyntax Syntax { get; }

    public CSharpStruct(StructDeclarationSyntax syntax)
        : base(syntax)
    {
        Syntax = syntax;
    }
}
