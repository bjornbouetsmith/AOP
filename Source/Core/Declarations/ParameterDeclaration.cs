using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core.Declarations
{
    public readonly record struct ParameterDeclaration(string TypeName, string ParameterName, bool IsThis, bool IsCallerMemberName)
    {
        public override string ToString()
        {
            return ParameterSyntax.ToFullString();
        }

        public ParameterSyntax ParameterSyntax
        {
            get
            {
                var type = SyntaxFactory.ParseTypeName(TypeName);
                var identifier = SyntaxFactory.Identifier(ParameterName);
                var parameter = SyntaxFactory.Parameter(identifier).WithType(type);
                if (IsThis)
                {
                    var thisModifier = SyntaxFactory.Token(SyntaxKind.ThisKeyword);
                    parameter = parameter.AddModifiers(thisModifier);
                }

                if (IsCallerMemberName && !IsThis)
                {
                    var attribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("System.Runtime.CompilerServices.CallerMemberName"));
                    var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute));
                    parameter = parameter.AddAttributeLists(attributeList);
                    parameter = parameter.WithDefault(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(""))));
                }
                return parameter.NormalizeWhitespace();
            }
        }
    }
    //https://roslynquoter.azurewebsites.net/
}