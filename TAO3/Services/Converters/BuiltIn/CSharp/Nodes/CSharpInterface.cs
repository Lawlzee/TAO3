using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public class CSharpInterface : CSharpBaseTypeDeclaration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new InterfaceDeclarationSyntax Syntax { get; }

    public CSharpInterface(InterfaceDeclarationSyntax syntax)
        : base(syntax)
    {
        Syntax = syntax;
    }
}
