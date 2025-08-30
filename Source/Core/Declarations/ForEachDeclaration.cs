using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.Declarations
{
    public readonly record struct ForEachDeclaration(string Identifier, MemberAccessDeclaration In, BlockDeclaration Block)
    {
        public StatementSyntax Syntax =>

                SyntaxFactory.ForEachStatement(
                    SyntaxFactory.IdentifierName(
                        SyntaxFactory.Identifier(
                            SyntaxFactory.TriviaList(),
                            SyntaxKind.VarKeyword,
                            "var",
                            "var",
                            SyntaxFactory.TriviaList())),
                    SyntaxFactory.Identifier(Identifier),
                    In.Syntax,
                    Block.Syntax);
    }

    public readonly record struct BlockDeclaration(params StatementSyntax[] Statements)
    {
        public BlockSyntax Syntax => SyntaxFactory.Block(Statements);
    }
}
