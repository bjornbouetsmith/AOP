using Core.Declarations;
using Xunit.Abstractions;

namespace AOPTest
{
    public class SimpleExpressionDeclarationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SimpleExpressionDeclarationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void SimpleExpressionTest() 
        {
            var leftSide = new MemberAccessDeclaration("obj1", "PropertyA");
            var rightSide = new MemberAccessDeclaration("obj1", "PropertyA");
            var assignment = new AssignmentDeclaration(leftSide, rightSide);

            SimpleExpressionDeclaration declaration = new SimpleExpressionDeclaration(assignment);
            var result = declaration.ExpressionStatement.ToFullString();
            _testOutputHelper.WriteLine(result);
        }
    }
}
