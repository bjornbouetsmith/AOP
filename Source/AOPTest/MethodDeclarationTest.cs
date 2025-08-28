using Core.Declarations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Public;
using Xunit.Abstractions;

namespace AOPTest
{
    public class MethodDeclarationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MethodDeclarationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void SimpleMethodTest()
        {
            var method = new MethodDeclaration("Copy", AccessModifier.Public | AccessModifier.Static,
                new SmallList<ParameterDeclaration>(

                    new ParameterDeclaration("TIn", "source", false, false),
                    new ParameterDeclaration("TOut", "destination", false, false)
                ),
                new SmallList<ParameterDeclaration>
                (
                    new ParameterDeclaration("TIn", "TIn", false, false),
                    new ParameterDeclaration("TOut", "TOut", false, false)
                ), null);

            var result = method.MethodDeclarationSyntax.ToFullString();
            _testOutputHelper.WriteLine(result);
        }

        [Fact]
        public void WithBodyTest()
        {
            var statement = new StatementDeclaration(new SimpleExpressionDeclaration(new AssignmentDeclaration(
                    new MemberAccessDeclaration("destination", "Property"),
                    new MemberAccessDeclaration("source", "Property")
                )));
            var body = new BodyDeclaration(statement.StatementSyntax);

            var method = new MethodDeclaration("Copy", AccessModifier.Public | AccessModifier.Static,
                new SmallList<ParameterDeclaration>(

                    new ParameterDeclaration("TIn", "source", false, false),
                    new ParameterDeclaration("TOut", "destination", false, false)
                ),
                new SmallList<ParameterDeclaration>
                (
                    new ParameterDeclaration("TIn", "TIn", false, false),
                    new ParameterDeclaration("TOut", "TOut", false, false)
                ), body);
            var result = method.MethodDeclarationSyntax.ToFullString();
            _testOutputHelper.WriteLine(result);
        }

        [Fact]
        public void ParseTest()
        {
            var options = new CSharpParseOptions(LanguageVersion.CSharp13, kind: SourceCodeKind.Regular);
            var code = "public static class MyClass{ public static string Hello(this string who)=>\"Hello \"+who;}";
            var statement = CSharpSyntaxTree.ParseText(code, options: options);

            var result = statement.
                GetRoot().NormalizeWhitespace().ToFullString();

            
            _testOutputHelper.WriteLine(result);
        }


    }
}
