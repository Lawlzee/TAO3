using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.CSharp
{
    public class CSharpCompilationUnit : ICSharpNode 
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public CompilationUnitSyntax Syntax { get; }
        CSharpSyntaxNode ICSharpNode.Syntax => Syntax;

        public IReadOnlyList<string> Usings { get; }
        public IReadOnlyList<CSharpAttribute> Attributes { get; }

        public IReadOnlyList<ICSharpMember> Members { get; }
        public IReadOnlyList<ICSharpTypeDeclaration> Types { get; }

        public IReadOnlyList<CSharpClass> Classes { get; }
        public IReadOnlyList<CSharpStruct> Structs { get; }
        public IReadOnlyList<CSharpInterface> Interfaces { get; }
        public IReadOnlyList<CSharpRecord> Records { get; }

        public IReadOnlyList<CSharpEnum> Enums { get; }

        public IReadOnlyList<CSharpField> Fields { get; }
        public IReadOnlyList<CSharpProperty> Properties { get; }
        public IReadOnlyList<CSharpMethod> Methods { get; }

        public CSharpCompilationUnit(CompilationUnitSyntax syntax)
        {
            Syntax = syntax;
            Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
            Usings = syntax.Usings
                .Select(x => x.Name.ToString())
                .ToList();

            Members = syntax.Members
                .SelectMany(x => ICSharpMember.Create(x))
                .ToList();

            Types = Members.OfType<ICSharpTypeDeclaration>().ToList();
            Classes = Types.OfType<CSharpClass>().ToList();
            Structs = Types.OfType<CSharpStruct>().ToList();
            Interfaces = Types.OfType<CSharpInterface>().ToList();
            Records = Types.OfType<CSharpRecord>().ToList();

            Enums = Types.OfType<CSharpEnum>().ToList();

            Fields = Members.OfType<CSharpField>().ToList();
            Properties = Members.OfType<CSharpProperty>().ToList();
            Methods = Members.OfType<CSharpMethod>().ToList();

        }
    }
}
