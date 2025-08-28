using Public;

namespace AOPTest
{
    public class SmallListTests
    {
        [Fact]
        public void EmptyList_HasCountZero()
        {
            var list = new SmallList<int>();
            Assert.Empty(list);
        }

        [Fact]
        public void SingleItemList_HasCountOne_AndContainsItem()
        {
            var list = new SmallList<string>("hello");
            Assert.Single(list);
            Assert.Contains("hello", list);
        }

        [Fact]
        public void TwoItemList_HasCountTwo_AndContainsItems()
        {
            var list = new SmallList<int>(1, 2);
            Assert.Equal(2, list.Count);
            Assert.Contains(1, list);
            Assert.Contains(2, list);
        }

        [Fact]
        public void ThreeItemList_HasCountThree_AndContainsItems()
        {
            var list = new SmallList<int>(1, 2, 3);
            Assert.Equal(3, list.Count);
            Assert.Contains(1, list);
            Assert.Contains(2, list);
            Assert.Contains(3, list);
        }

        [Fact]
        public void Add_ThrowsWhenExceedingCapacity()
        {
            var list = new SmallList<int>(1, 2, 3);
            Assert.Throws<InvalidOperationException>(() => list.Add(4));
        }

        [Fact]
        public void Enumerator_IteratesCorrectly()
        {
            var list = new SmallList<int>(10, 20, 30);
            var items = new List<int>();
            foreach (var item in list)
                items.Add(item);

            Assert.Equal(new[] { 10, 20, 30 }, items);
        }

        [Fact]
        public void Equality_WorksForSameItems()
        {
            var a = new SmallList<int>(1, 2, 3);
            var b = new SmallList<int>(1, 2, 3);
            Assert.True(a == b);
            Assert.True(a.Equals(b));
        }

        [Fact]
        public void Equality_FailsForDifferentItems()
        {
            var a = new SmallList<int>(1, 2, 3);
            var b = new SmallList<int>(1, 2, 4);
            Assert.False(a == b);
            Assert.False(a.Equals(b));
        }
    }
}
