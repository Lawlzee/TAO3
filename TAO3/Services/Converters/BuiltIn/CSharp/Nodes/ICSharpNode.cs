using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.CSharp
{
    public interface ICSharpNode
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        CSharpSyntaxNode Syntax { get; }
    }
}
