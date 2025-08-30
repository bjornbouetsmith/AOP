using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core.Declarations
{
    public readonly record struct MemberAccessDeclaration(string Expression, string? MemberName = null)
    {
        public static implicit operator MemberAccessDeclaration(string statement)
        {
            var syntaxNode = CSharpSyntaxTree.ParseText(statement).GetRoot();
            return syntaxNode is MemberAccessExpressionSyntax memberAccess ? new MemberAccessDeclaration(memberAccess.Expression.ToString(), memberAccess.Name.ToString()) : throw new System.ArgumentException("Invalid member access expression", nameof(statement));
        }

        public ExpressionSyntax Syntax
        {
            get
            {
                if (MemberName != null)
                {
                    return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(Expression), SyntaxFactory.IdentifierName(MemberName));
                }

                return SyntaxFactory.IdentifierName(Expression);
            }
        }
    }
}
