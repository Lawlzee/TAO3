using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public class CSharpClass : CSharpBaseTypeDeclaration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new ClassDeclarationSyntax Syntax { get; }

    public CSharpClass(ClassDeclarationSyntax syntax)
        : base(syntax)
    {
        Syntax = syntax;
    }
}
