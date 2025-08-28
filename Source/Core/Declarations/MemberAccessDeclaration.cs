using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core.Declarations
{
    public readonly record struct MemberAccessDeclaration(string Expression, string MemberName) 
    {
        public static implicit operator MemberAccessDeclaration(string statement)=>CSharpSyntaxTree.ParseText(statement).GetRoot() is MemberAccessExpressionSyntax memberAccess ? new MemberAccessDeclaration(memberAccess.Expression.ToString(), memberAccess.Name.ToString()) : throw new System.ArgumentException("Invalid member access expression", nameof(statement));
    }
}
