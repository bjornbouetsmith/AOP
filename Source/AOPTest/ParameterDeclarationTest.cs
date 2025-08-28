using Core.Declarations;
using Xunit.Abstractions;

using Public;

namespace AOPTest
{
    public class ParameterDeclarationTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ParameterDeclarationTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("int", "x", false, "int x")]
        [InlineData("string", "name", true, "this string name")]
        [InlineData("double", "value", false, "double value")]
        [InlineData("bool", "flag", true, "this bool flag")]
        public void ToString_ReturnsExpectedFormat(string typeName, string parameterName, bool isThis, string expected)
        {
            var param = new ParameterDeclaration(typeName, parameterName, isThis, false);
            _testOutputHelper.WriteLine($"Parameter: {param}");
            Assert.Equal(expected, param.ToString());
        }

        [Fact]
        public void SyntaxShouldMatchDeclaration()
        {
            var declaration = new ParameterDeclaration("System.String", "caller", false, true);

            var fullString = declaration.ParameterSyntax.ToFullString();
            _testOutputHelper.WriteLine(fullString);
            Assert.Equal("[System.Runtime.CompilerServices.CallerMemberName] System.String caller = \"\"", fullString);
        }
    }
}
