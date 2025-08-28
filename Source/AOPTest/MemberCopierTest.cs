using Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace AOPTest
{
    public class MemberCopierTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MemberCopierTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private const string Data = @"

using System;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class MemberCopierAttribute : Attribute
    {
        public Type DestinationType { get; set; }
    }
namespace MyNamespace
{
    [MemberCopier(DestinationType = typeof(MyCopy))]
    [MemberCopier(DestinationType = typeof(MyClass))]
    public class MyClass 
    {
        public string MyProperty { get; set; }
        public int MyProperty1 { get; set; }
        public MyClass Source { get; set; }
    }

    public class MyCopy 
    {
        public string MyProperty { get; set; }
        public int MyProperty1 { get; set; }
        public MyClass Source { get; set; }
    }
}
";
        [Fact]
        public void TestMemberCopierSimple()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(Data);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", new[] { syntaxTree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var syntaxRoot = syntaxTree.GetRoot();
            var myClass = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First(cd => cd.Identifier.ValueText == "MyClass");

            var classModel = semanticModel.GetDeclaredSymbol(myClass!);


            var copier = MemberCopier.CreateCopierClass(classModel!);

            _testOutputHelper.WriteLine(copier.NormalizeWhitespace().ToFullString());

        }

    }
}
