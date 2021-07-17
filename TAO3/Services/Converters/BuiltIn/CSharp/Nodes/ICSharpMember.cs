using Microsoft.CodeAnalysis;
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
    public interface ICSharpMember : ICSharpNode
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        new MemberDeclarationSyntax Syntax { get; }
        CSharpSyntaxNode ICSharpNode.Syntax => Syntax;

        IReadOnlyList<CSharpAttribute> Attributes { get; }
        CSharpModifiers Modifiers { get; }
        string Name { get; }

        public static IEnumerable<ICSharpMember> Create(MemberDeclarationSyntax syntax, string? @namespace = null)
        {
            return syntax switch
            {
                ClassDeclarationSyntax x => One(new CSharpClass(x)),
                RecordDeclarationSyntax x => One(new CSharpRecord(x)),
                InterfaceDeclarationSyntax x => One(new CSharpInterface(x)),
                StructDeclarationSyntax x => One(new CSharpStruct(x)),
                EnumDeclarationSyntax x => One(new CSharpEnum(x)),
                PropertyDeclarationSyntax x => One(new CSharpProperty(x)),
                FieldDeclarationSyntax x => One(new CSharpField(x)),
                EnumMemberDeclarationSyntax x => One(new CSharpEnumMember(x)),
                MethodDeclarationSyntax x => One(new CSharpMethod(x)),
                ConstructorDeclarationSyntax x => One(new CSharpConstructor(x)),
                NamespaceDeclarationSyntax x => x.Members.SelectMany(y => Create(y, x.Name.ToString())),
                _ => Array.Empty<ICSharpMember>()
            };

            IEnumerable<ICSharpMember> One(ICSharpMember value)
            {
                yield return value;
            }
        }
    }
}
