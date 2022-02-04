using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public interface ICSharpTypeDeclaration : ICSharpMember
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    new BaseTypeDeclarationSyntax Syntax { get;}
    IReadOnlyList<CSharpType> Parents { get; }
    IReadOnlyList<ICSharpMember> Members { get; }
}
