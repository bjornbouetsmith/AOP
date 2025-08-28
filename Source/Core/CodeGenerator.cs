using System;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core
{
    //[Generator(LanguageNames.CSharp)]
    //public class EnumExtGeneratpr : AttributeGenerator<EnumExtAttribute>;
    [Generator(LanguageNames.CSharp)]
    public class MemberCopierGeneratpr : AttributeGenerator<MemberCopierAttribute>;

    public class AttributeGenerator<TAttribute> : IIncrementalGenerator
        where TAttribute : Attribute
    {
        private readonly string _fullyQualifiedName = typeof(TAttribute).FullName;

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            var valuesProvider =
            context.SyntaxProvider.ForAttributeWithMetadataName(
                _fullyQualifiedName,
                static (node, ct) => CouldBeEnumerationAsync(node, ct),
                static (ctx, ct) => GetEnumData(ctx, ct));

            var notNull = valuesProvider
                .WithTrackingName("InitialExtraction")
                .Where(static m => m is not null)
                .Select(static (m, _) => m!.Value)
                .WithTrackingName("RemovingNulls");

            context.RegisterSourceOutput(notNull, static (spc, meth) => Execute(meth, spc));
        }
        private static bool CouldBeEnumerationAsync(
            SyntaxNode syntaxNode,
            CancellationToken cancellationToken)
        {
            return syntaxNode is EnumDeclarationSyntax;
        }

        private static EnumData? GetEnumData(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            INamedTypeSymbol? enumSymbol = context.TargetSymbol as INamedTypeSymbol;
            if (enumSymbol is null)
            {
                // nothing to do if this type isn't available
                return null;
            }
            
            var type = (EnumDeclarationSyntax)context.TargetNode;

            SyntaxList<AttributeListSyntax> attributes = type.AttributeLists;
            var enumName = enumSymbol.Name;
            var ns = enumSymbol.ContainingNamespace.ToDisplayString();
            //type.Identifier.Text
            return new EnumData(context.TargetNode.GetLocation(), enumName, ns, attributes);
        }

        private static void Execute(in EnumData enumData, SourceProductionContext context)
        {
            //var descriptor = new DiagnosticDescriptor("BBS001", "Output", "Adding enum Extension methods for {0}", "BBS", DiagnosticSeverity.Info, true, description: null, helpLinkUri: "https://r00t.dk/aop/bbs001");
            //context.ReportDiagnostic(Diagnostic.Create(descriptor, enumData.Location, enumData.EnumName));
            //return;
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}
            var generator = new EnumExtGenerator(enumData, context);
            var result = generator.Execute();
            if (result != null)
            {
                context.AddSource($"{enumData.NameSpace}_{enumData.EnumName}_EnumExt.g.cs", result.ToFullString());
            }
        }
    }


    public readonly record struct EnumData(Location Location, string EnumName, string NameSpace, SyntaxList<AttributeListSyntax> Attributes);


    public class EnumExtGenerator
    {
        private readonly DiagnosticDescriptor _descriptor
            = new DiagnosticDescriptor("BBS001", "Output", "Adding enum Extension methods for {0}", "BBS", DiagnosticSeverity.Info, true, description: null, helpLinkUri: "https://r00t.dk/aop/bbs001");
        private readonly EnumData _enumData;
        private readonly SourceProductionContext _context;

        public EnumExtGenerator(in EnumData enumData, SourceProductionContext context)
        {
            _enumData = enumData;
            _context = context;
        }

        public SyntaxNode? Execute()
        {
            _context.ReportDiagnostic(Diagnostic.Create(_descriptor, _enumData.Location, _enumData.EnumName));

            //return null;

            var list = SyntaxFactory.List<MemberDeclarationSyntax>([]);
            var usingSystem = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System"))
                .WithTrailingTrivia(SyntaxFactory.Space)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                      
            
            
            var ns = CreateNamespace();
            
            

            var classDeclation = SyntaxFactory.ClassDeclaration($"{_enumData.EnumName}Extensions")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddMembers(CreateAsStringMethod())
                .NormalizeWhitespace();


            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(ns)
                .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken)
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed))
                .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                    .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed))
                .AddMembers(classDeclation)
                .NormalizeWhitespace();
            return namespaceDeclaration;

            //SyntaxTokenList modifiers = SyntaxFactory.TokenList(
            //    usingSystem, semicolon, semicolon,
            //    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            //    SyntaxFactory.Token(SyntaxKind.StaticKeyword));

            //var namespaceDeclation  SyntaxFactory.NamespaceDeclaration(ns,


            //var usings = SyntaxFactory.UsingDirective(
            //     SyntaxFactory.Token(SyntaxKind.UsingKeyword)
            //     null,
            //     SyntaxFactory.IdentifierName("System"),
            //     SyntaxFactory.Token(SyntaxKind.SemicolonToken));


            //var classDeclaration = SyntaxFactory.ClassDeclaration($"{_enumData.EnumName}Extensions")
            //    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            //NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("Core"))

            //    .AddMembers(classDeclaration).AddUsings(usings);
            //return namespaceDeclaration;
            //return members;
        }

        private NameSyntax CreateNamespace()
        {
            return SyntaxFactory.IdentifierName(_enumData.NameSpace);
        }
        private MethodDeclarationSyntax CreateAsStringMethod() 
        {
            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), "AsString")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                        .WithType(SyntaxFactory.IdentifierName(_enumData.EnumName))
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword))))
                .WithBody(
                    SyntaxFactory.Block(
                        SyntaxFactory.SingletonList<StatementSyntax>(
                            SyntaxFactory.ReturnStatement(
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("value"),
                                        SyntaxFactory.IdentifierName("ToString")))
                                .WithArgumentList(SyntaxFactory.ArgumentList())))))
                ;
            return method;
        }
    }
}