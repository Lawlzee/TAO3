using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.CSharp
{
    public interface ICSharpTypeDeclaration : ICSharpMember
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        new BaseTypeDeclarationSyntax Syntax { get;}
        IReadOnlyList<CSharpType> Parents { get; }
        IReadOnlyList<ICSharpMember> Members { get; }
    }
}
