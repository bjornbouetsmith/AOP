using Xunit.Abstractions;

namespace AOPTest
{
    public class AssignmentDeclationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AssignmentDeclationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void SimpleDeclationTest() 
        {
            var leftSide = new Core.Declarations.MemberAccessDeclaration("obj1", "PropertyA");
            var rightSide = new Core.Declarations.MemberAccessDeclaration("obj1", "PropertyA");
            var assignment = new Core.Declarations.AssignmentDeclaration(leftSide, rightSide);
            var result = assignment.AssignmentExpression.ToFullString();
            _testOutputHelper.WriteLine(result);
        }
    }
}
