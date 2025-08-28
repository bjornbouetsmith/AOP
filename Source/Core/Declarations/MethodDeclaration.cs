using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Public;
using System.Linq;

namespace Core.Declarations
{
    public readonly record struct StatementDeclaration(SimpleExpressionDeclaration Declaration)
    {
        public StatementSyntax StatementSyntax => Declaration.ExpressionStatement;

        public override string ToString()
        {
            return StatementSyntax.ToFullString();
        }
    }
    
    public readonly record struct InvocationStatementDeclaration(InvocationExpressionDeclaration Declaration)
    {
        public StatementSyntax StatementSyntax => Declaration.ExpressionStatement;

        public override string ToString()
        {
            return StatementSyntax.ToFullString();
        }
    }

    public readonly record struct BodyDeclaration(params StatementSyntax[] Statements)
    {
        public override string ToString()
        {
            return BodySyntax?.ToFullString() ?? string.Empty;
        }
        public BlockSyntax? BodySyntax => Statements != null ? SyntaxFactory.Block(Statements) : null;
    }

    public readonly struct MethodDeclaration(string Name, AccessModifier AccessModifier, SmallList<ParameterDeclaration> Parameters, SmallList<ParameterDeclaration> GenericParameters, BodyDeclaration? Body)
    {

        public MethodDeclarationSyntax MethodDeclarationSyntax
        {
            get
            {
                var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), Name)
                    .WithModifiers(GetModifiers())
                    .WithParameterList(GetParameterList())
                    .WithTypeParameterList(GetTypeParameterList())
                    .WithBody(Body?.BodySyntax);

                //var method = SyntaxFactory.MethodDeclaration(
                //    attributeLists: new SyntaxList<AttributeListSyntax>(),
                //    modifiers: SyntaxFactory.TokenList(GetAccessModifierTokens()),
                //    returnType: SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                //    explicitInterfaceSpecifier: null,
                //    identifier: SyntaxFactory.Identifier(Name),
                //    typeParameterList: GetTypeParameterList(),
                //    parameterList: GetParameterList(),
                //    constraintClauses: new SyntaxList<TypeParameterConstraintClauseSyntax>(),
                //    body: Body?.BodySyntax,
                //    expressionBody: null,
                //    semicolonToken: SyntaxFactory.Token(SyntaxKind.None)
                //);
                return method.NormalizeWhitespace();
            }
        }

        private SyntaxTokenList GetModifiers()
        {
            var xmlNode = CSharpSyntaxTree.ParseText(@"/// <summary>
	/// Copies all property values from source to destination
	/// </summary>
").GetRoot();
            var xmlDoc = xmlNode.ChildTokens();
            //var comment = SyntaxFactory.Token(SyntaxFactory.TriviaList(
            //    SyntaxFactory.Comment("// Compiler generated")), SyntaxKind.None, SyntaxFactory.TriviaList());
            var eol = SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed), SyntaxKind.None, SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed));
            
            var baseModifier = SyntaxFactory.TokenList(xmlDoc).Add(eol);
            
            //baseModifier = baseModifier.Add(eol);
            //foreach (var token in xmlDoc)
            //{
            //    baseModifier = baseModifier.Add(token);
            //}
            //baseModifier = baseModifier.Add(eol);
            //baseModifier = baseModifier.Add(eol);


            if ((AccessModifier & AccessModifier.Public) == AccessModifier.Public)
            {
                baseModifier = baseModifier.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            }
            else if ((AccessModifier & AccessModifier.Private) == AccessModifier.Private)
            {
                baseModifier = baseModifier.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            }
            else if ((AccessModifier & AccessModifier.Protected) == AccessModifier.Protected)
            {
                baseModifier = baseModifier.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
            }
            else if ((AccessModifier & AccessModifier.Internal) == AccessModifier.Internal)
            {
                baseModifier = baseModifier.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
            }

            if (AccessModifier.HasFlag(AccessModifier.Static))
            {
                return baseModifier.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }

            return SyntaxFactory.TokenList(baseModifier);

        }

        private ParameterListSyntax GetParameterList()
        {
            var list = SyntaxFactory.NodeOrTokenList();
            for (int x = 0; x < Parameters.Count; x++)
            {
                list = list.Add(Parameters[x].ParameterSyntax);
                if (x < Parameters.Count - 1)
                {
                    list = list.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                }
            }
            return SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(list));
        }

        private TypeParameterListSyntax? GetTypeParameterList()
        {
            if(GenericParameters.Count == 0)
            {
                return null;
            }
            var genericParameters = SyntaxFactory.NodeOrTokenList();
            for (int x = 0; x < GenericParameters.Count; x++)
            {
                genericParameters = genericParameters.Add(SyntaxFactory.TypeParameter(SyntaxFactory.Identifier(GenericParameters[x].TypeName)));
                if (x < GenericParameters.Count - 1)
                {
                    genericParameters = genericParameters.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                }
            }

            return SyntaxFactory.TypeParameterList(SyntaxFactory.SeparatedList<TypeParameterSyntax>(genericParameters));

        }
    }
}