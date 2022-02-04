using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public interface ICSharpNode
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    CSharpSyntaxNode Syntax { get; }
}
