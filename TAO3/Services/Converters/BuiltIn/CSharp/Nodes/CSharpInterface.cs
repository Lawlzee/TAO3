using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.CSharp
{
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
}
