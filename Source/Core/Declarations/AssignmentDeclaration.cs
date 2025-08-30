using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core.Declarations
{
    public readonly record struct AssignmentDeclaration(MemberAccessDeclaration leftSide, MemberAccessDeclaration rightSide)
    {
        public ExpressionSyntax AssignmentExpression => SyntaxFactory.AssignmentExpression
        (
            SyntaxKind.SimpleAssignmentExpression,
            SyntaxFactory.MemberAccessExpression
            (
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(leftSide.Expression),
                SyntaxFactory.IdentifierName(leftSide.MemberName)
            ),
            SyntaxFactory.MemberAccessExpression
            (
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(rightSide.Expression),
                SyntaxFactory.IdentifierName(rightSide.MemberName)
            )
        );
    }

    public readonly record struct InvocationDeclaration(MemberAccessDeclaration Member, string method, params MemberAccessDeclaration[] arguments)
    {
        public InvocationExpressionSyntax InvocationExpression
        {
            get
            {
                var convertedArguments = arguments.Select(a =>
                 SyntaxFactory.Argument
                 (
                     a.Syntax
                 ));
                var argumentList = SyntaxFactory.SeparatedList(convertedArguments);
                var invocation = SyntaxFactory.InvocationExpression
                (
                    SyntaxFactory.MemberAccessExpression
                    (
                        SyntaxKind.SimpleMemberAccessExpression,
                        Member.Syntax,
                        SyntaxFactory.IdentifierName(method)
                    ),
                    SyntaxFactory.ArgumentList(argumentList)
                );
                return invocation;
            }
        }
    }
}