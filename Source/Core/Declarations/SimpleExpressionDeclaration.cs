using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core.Declarations
{
    public readonly record struct SimpleExpressionDeclaration(AssignmentDeclaration assignment) 
    {
        public StatementSyntax ExpressionStatement => SyntaxFactory.ExpressionStatement(assignment.AssignmentExpression);
    }

    public readonly record struct InvocationExpressionDeclaration(InvocationDeclaration assignment)
    {
        public StatementSyntax ExpressionStatement => SyntaxFactory.ExpressionStatement(assignment.InvocationExpression);
    }
}
