using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public class CSharpRecord : CSharpBaseTypeDeclaration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new RecordDeclarationSyntax Syntax { get; }
    public IReadOnlyList<CSharpParameter> Parameters { get; }

    public CSharpRecord(RecordDeclarationSyntax syntax)
        : base(syntax)
    {
        Syntax = syntax;
        Parameters = syntax.ParameterList == null
            ? new List<CSharpParameter>()
            : syntax.ParameterList
                .Parameters
                .Select(p => new CSharpParameter(p))
                .ToList();
    }
}
