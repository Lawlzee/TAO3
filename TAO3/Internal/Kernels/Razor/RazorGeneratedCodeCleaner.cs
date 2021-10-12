using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Types;

namespace TAO3.Internal.Kernels.Razor
{
    //todo: rename
    internal class RazorGeneratedCodeCleaner : CSharpSyntaxRewriter
    {
        private readonly Dictionary<string, Type> _variables;

        private RazorGeneratedCodeCleaner(Dictionary<string, Type> variables, bool visitIntoStructuredTrivia = false) : base(visitIntoStructuredTrivia)
        {
            _variables = variables;
        }

        public static string Clean(string code, Dictionary<string, Type> variables)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);

            return new RazorGeneratedCodeCleaner(variables)
                .Visit(tree.GetRoot())
                .ToFullString();
        }

        public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node)
        {
            return base.VisitCompilationUnit(node.WithAttributeLists(SyntaxFactory.List<AttributeListSyntax>()));
        }
        
        public override SyntaxNode? VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            //Take the class in the namespace
            return Visit(node.Members[0]);
        }

        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.Identifier.ValueText != "GeneratedTemplate")
            {
                return base.VisitClassDeclaration(node);
            }

            List<MemberDeclarationSyntax> newMembers = new List<MemberDeclarationSyntax>();
            /*
            private readonly Microsoft.DotNet.Interactive.CSharp.CSharpKernel _csharpKernel;
            */

            FieldDeclarationSyntax field = SyntaxFactory
                .FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.QualifiedName(
                                    SyntaxFactory.QualifiedName(
                                        SyntaxFactory.IdentifierName("Microsoft"),
                                        SyntaxFactory.IdentifierName("DotNet")),
                                    SyntaxFactory.IdentifierName("Interactive")),
                                SyntaxFactory.IdentifierName("CSharp")),
                            SyntaxFactory.IdentifierName(
                                SyntaxFactory.Identifier(
                                    SyntaxFactory.TriviaList(),
                                    "CSharpKernel",
                                    SyntaxFactory.TriviaList(
                                        SyntaxFactory.Space)))))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier("_csharpKernel")))))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        new[]{
                            SyntaxFactory.Token(
                                SyntaxFactory.TriviaList(
                                    new []{
                                        SyntaxFactory.LineFeed,
                                        SyntaxFactory.Whitespace("        ")}),
                                SyntaxKind.PrivateKeyword,
                                SyntaxFactory.TriviaList(
                                    SyntaxFactory.Space)),
                            SyntaxFactory.Token(
                                SyntaxFactory.TriviaList(),
                                SyntaxKind.ReadOnlyKeyword,
                                SyntaxFactory.TriviaList(
                                    SyntaxFactory.Space))}))
                .WithSemicolonToken(
                    SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(),
                        SyntaxKind.SemicolonToken,
                        SyntaxFactory.TriviaList(
                            SyntaxFactory.LineFeed)));

            newMembers.Add(field);

            /*
                public GeneratedTemplate(Microsoft.DotNet.Interactive.CSharp.CSharpKernel csharpKernel)
                {
                    _csharpKernel = csharpKernel;
                }
            */
            ConstructorDeclarationSyntax constructor = SyntaxFactory
                .ConstructorDeclaration(
                    SyntaxFactory.Identifier("GeneratedTemplate"))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Whitespace("        ")),
                            SyntaxKind.PublicKeyword,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Space))))
                .WithParameterList(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                            SyntaxFactory.Parameter(
                                SyntaxFactory.Identifier("csharpKernel"))
                            .WithType(
                                SyntaxFactory.QualifiedName(
                                    SyntaxFactory.QualifiedName(
                                        SyntaxFactory.QualifiedName(
                                            SyntaxFactory.QualifiedName(
                                                SyntaxFactory.IdentifierName("Microsoft"),
                                                SyntaxFactory.IdentifierName("DotNet")),
                                            SyntaxFactory.IdentifierName("Interactive")),
                                        SyntaxFactory.IdentifierName("CSharp")),
                                    SyntaxFactory.IdentifierName(
                                        SyntaxFactory.Identifier(
                                            SyntaxFactory.TriviaList(),
                                            "CSharpKernel",
                                            SyntaxFactory.TriviaList(
                                                SyntaxFactory.Space)))))))
                    .WithCloseParenToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(),
                            SyntaxKind.CloseParenToken,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.CarriageReturnLineFeed))))
                .WithBody(
                    SyntaxFactory.Block(
                        SyntaxFactory.SingletonList<StatementSyntax>(
                            SyntaxFactory.ExpressionStatement(
                                SyntaxFactory.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.IdentifierName(
                                        SyntaxFactory.Identifier(
                                            SyntaxFactory.TriviaList(
                                                SyntaxFactory.Whitespace("            ")),
                                            "_csharpKernel",
                                            SyntaxFactory.TriviaList(
                                                SyntaxFactory.Space))),
                                    SyntaxFactory.IdentifierName("csharpKernel"))
                                .WithOperatorToken(
                                    SyntaxFactory.Token(
                                        SyntaxFactory.TriviaList(),
                                        SyntaxKind.EqualsToken,
                                        SyntaxFactory.TriviaList(
                                            SyntaxFactory.Space))))
                            .WithSemicolonToken(
                                SyntaxFactory.Token(
                                    SyntaxFactory.TriviaList(),
                                    SyntaxKind.SemicolonToken,
                                    SyntaxFactory.TriviaList(
                                        SyntaxFactory.CarriageReturnLineFeed)))))
                    .WithOpenBraceToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Whitespace("        ")),
                            SyntaxKind.OpenBraceToken,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.CarriageReturnLineFeed)))
                    .WithCloseBraceToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Whitespace("        ")),
                            SyntaxKind.CloseBraceToken,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.CarriageReturnLineFeed))));

            newMembers.Add(constructor);

            foreach ((string name, Type type) in _variables)
            {
                /*
                    private $type $name => _csharpKernel.TryGetValue("$name", out $type value) ? value : default;
                */

                string fullType = type.PrettyPrintFullName(anymousClassAsDynamic: true);

                PropertyDeclarationSyntax property = SyntaxFactory
                    .PropertyDeclaration(
                        SyntaxFactory.IdentifierName(
                            SyntaxFactory.Identifier(
                                SyntaxFactory.TriviaList(),
                                fullType,
                                SyntaxFactory.TriviaList(
                                    SyntaxFactory.Space))),
                        SyntaxFactory.Identifier(
                            SyntaxFactory.TriviaList(),
                            name,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Space)))
                    .WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.Token(
                                SyntaxFactory.TriviaList(
                                    new[]{
                                        SyntaxFactory.LineFeed,
                                        SyntaxFactory.Whitespace("        ")}),
                                SyntaxKind.PrivateKeyword,
                                SyntaxFactory.TriviaList(
                                    SyntaxFactory.Space))))
                    .WithExpressionBody(
                        SyntaxFactory.ArrowExpressionClause(
                            SyntaxFactory.ConditionalExpression(
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("_csharpKernel"),
                                        SyntaxFactory.IdentifierName("TryGetValue")))
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]{
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.LiteralExpression(
                                                        SyntaxKind.StringLiteralExpression,
                                                        SyntaxFactory.Literal(name))),
                                                SyntaxFactory.Token(
                                                    SyntaxFactory.TriviaList(),
                                                    SyntaxKind.CommaToken,
                                                    SyntaxFactory.TriviaList(
                                                        SyntaxFactory.Space)),
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.DeclarationExpression(
                                                        SyntaxFactory.IdentifierName(
                                                            SyntaxFactory.Identifier(
                                                                SyntaxFactory.TriviaList(),
                                                                fullType,
                                                                SyntaxFactory.TriviaList(
                                                                    SyntaxFactory.Space))),
                                                        SyntaxFactory.SingleVariableDesignation(
                                                            SyntaxFactory.Identifier("value"))))
                                                .WithRefOrOutKeyword(
                                                    SyntaxFactory.Token(
                                                        SyntaxFactory.TriviaList(),
                                                        SyntaxKind.OutKeyword,
                                                        SyntaxFactory.TriviaList(
                                                            SyntaxFactory.Space)))}))
                                    .WithCloseParenToken(
                                        SyntaxFactory.Token(
                                            SyntaxFactory.TriviaList(),
                                            SyntaxKind.CloseParenToken,
                                            SyntaxFactory.TriviaList(
                                                SyntaxFactory.Space)))),
                                SyntaxFactory.IdentifierName(
                                    SyntaxFactory.Identifier(
                                        SyntaxFactory.TriviaList(),
                                        "value",
                                        SyntaxFactory.TriviaList(
                                            SyntaxFactory.Space))),
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.DefaultLiteralExpression,
                                    SyntaxFactory.Token(SyntaxKind.DefaultKeyword)))
                            .WithQuestionToken(
                                SyntaxFactory.Token(
                                    SyntaxFactory.TriviaList(),
                                    SyntaxKind.QuestionToken,
                                    SyntaxFactory.TriviaList(
                                        SyntaxFactory.Space)))
                            .WithColonToken(
                                SyntaxFactory.Token(
                                    SyntaxFactory.TriviaList(),
                                    SyntaxKind.ColonToken,
                                    SyntaxFactory.TriviaList(
                                        SyntaxFactory.Space))))
                        .WithArrowToken(
                            SyntaxFactory.Token(
                                SyntaxFactory.TriviaList(),
                                SyntaxKind.EqualsGreaterThanToken,
                                SyntaxFactory.TriviaList(
                                    SyntaxFactory.Space))))
                    .WithSemicolonToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(),
                            SyntaxKind.SemicolonToken,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.LineFeed)));

                newMembers.Add(property);
            }

            return base.VisitClassDeclaration(node.WithMembers(node.Members.AddRange(newMembers)));
        }
    }
}
