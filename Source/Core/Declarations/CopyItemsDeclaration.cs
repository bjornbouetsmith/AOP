using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core.Declarations
{
    public readonly record struct CopyItemsDeclaration(MemberAccessDeclaration Source, MemberAccessDeclaration Target)
    {
        public StatementSyntax Syntax =>

            new ForEachDeclaration("item", Source, new BlockDeclaration(GetCopyAssignment())).Syntax;

        private StatementSyntax GetCopyAssignment()
        {
            var invocation = new InvocationDeclaration(Target, "Add", new MemberAccessDeclaration("item"));
            return SyntaxFactory.ExpressionStatement(invocation.InvocationExpression);
        }
    }
}