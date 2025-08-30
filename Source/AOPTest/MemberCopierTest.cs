using Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyNamespace;
using System.Security.AccessControl;
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
        [Fact]
        public void TestMemberCopierSimple()
        {
            var classModel = GetClassSymbol<MyClass>(typeof(MyCopy), typeof(MemberCopierAttribute));
            var attributes = classModel.NamedType!.GetAttributes();
            var attr = attributes.First(a => a.AttributeClass.Name == "MemberCopierAttribute");

            var copier = MemberCopier.CreateCopierClass(classModel.NamedType!, attr);

            _testOutputHelper.WriteLine(copier.NormalizeWhitespace().ToFullString());
            string expected = @"public static partial class Copier
{
    /// <summary>
    /// Copies all property values from source to destination
    /// </summary>
    public static void Copy(this MyNamespace.MyClass source, MyNamespace.MyCopy destination)
    {
        source.Source.CopyTo(destination.Source);
        destination.MyProperty = source.MyProperty;
        destination.MyProperty1 = source.MyProperty1;
    }
}";
            Assert.Equal(expected, copier.NormalizeWhitespace().ToFullString());
        }

        [Fact]
        public void MemberCopierShouldCopyCollections()
        {
            var classModel = GetClassSymbol<MyClassWithList>(typeof(List<>), typeof(MyDataClass), typeof(MyClassWithListDetination), typeof(MemberCopierAttribute));
            var attributes = classModel.NamedType!.GetAttributes();
            var attr = attributes.First(a => a.AttributeClass.Name == "MemberCopierAttribute");

            var copier = MemberCopier.CreateCopierClass(classModel.NamedType!, attr);

            _testOutputHelper.WriteLine(copier.NormalizeWhitespace().ToFullString());
        }

        [Fact]
        public void MemberCopierShouldCopyDictionaries()
        {
            Assert.Fail("Not Implemented");
        }

        private static (CSharpCompilation Compilation ,INamedTypeSymbol NamedType) GetClassSymbol<T>(params Type[] types)
        {
            var assemblies = GetTestAssemblies(types);
            CSharpCompilation compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: null, references: assemblies);
            var namedType = compilation.GetTypeByMetadataName(typeof(T).FullName!)!;
            return (compilation, namedType);
        }

        public class MyDataClass
        {
            public string StringData { get; set; }
            public int IntData { get; set; }
        }

        [MemberCopier(DestinationType = typeof(MyClassWithListDetination))]
        public class MyClassWithList
        {
            public List<string> Strings { get; set; }
            public List<int> Ints { get; set; }
            public List<MyDataClass> MyDatas { get; set; }
        }

        public class MyClassWithListDetination
        {
            public List<string> Strings { get; set; }
            public List<int> Ints { get; set; }
            public List<MyDataClass> MyDatas { get; set; }
        }

        private static PortableExecutableReference[] GetTestAssemblies(params Type[] testClasses) 
        {
            var baseAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location)).Select(assembly => assembly.Location);
            var testAssemblies = testClasses.Select(t => t.Assembly.Location);
            var allAssemblies = baseAssemblies.Concat(testAssemblies).Distinct();

            return allAssemblies.Select(l => MetadataReference.CreateFromFile(l)).ToArray();
        }

        public void Copy(ICollection<string> source, ICollection<string> destination)
        {
            foreach (var item in source)
            {
                destination.Add(item);
            }
            
        }   

    }
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


