using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.CSharp
{
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
}
