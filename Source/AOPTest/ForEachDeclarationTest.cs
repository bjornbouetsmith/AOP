using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Declarations;
using Microsoft.CodeAnalysis;
using Xunit.Abstractions;

namespace AOPTest
{
    public class ForEachDeclarationTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ForEachDeclarationTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void SimpleForEachTest() 
        {
            var memberAccess = new MemberAccessDeclaration("collection", "Items");
            var declaration = new ForEachDeclaration("item",memberAccess,new BlockDeclaration());

            _testOutputHelper.WriteLine(declaration.Syntax.NormalizeWhitespace().ToFullString());
        }
        [Fact]
        public void CopyItemsDeclarationTest() 
        {
            var source = new MemberAccessDeclaration("source", "Items");
            var target = new MemberAccessDeclaration("target", "Items");
            var copyItems = new CopyItemsDeclaration(source, target);
            _testOutputHelper.WriteLine(copyItems.Syntax.NormalizeWhitespace().ToFullString());

            ICollection<string> strings= null;

            strings ??= [];
            strings.Add("Yes");

        }
    }
}
