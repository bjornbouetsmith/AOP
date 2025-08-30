using Core;
using Xunit.Abstractions;

namespace AOPTest
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        //[Fact]
        //public void Test1()
        //{
        //    var data = MyEnum.Two;
        //    var result = data.AsString();
        //    _testOutputHelper.WriteLine(result);
        //}
    }
    //public static partial class Copier
    //{
    //    /// <summary>
    //    /// Copies all property values from source to destination
    //    /// </summary>
    //    public static void Copy(this MyClass source, MyCopy destination)
    //    {
    //        destination.MyProperty = source.MyProperty;
    //        destination.MyProperty1 = source.MyProperty1;
    //    }
    //}


    //[MemberCopier(DestinationType = typeof(MyCopy))]
    //public class MyClass
    //{
    //    public string MyProperty { get; set; }
    //    public int MyProperty1 { get; set; }
    //}

    //public class MyCopy
    //{
    //    public string MyProperty { get; set; }
    //    public int MyProperty1 { get; set; }
    //}

    public class MyTestClass
    {
        [PreCall]
        public void DoSomething() { }

        [PostCall]
        public void DoSomethingElse() { }
    }

    [EnumExt]
    public enum MyEnum
    {
        One,
        Two,
        Three
    }
}