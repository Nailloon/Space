using SpaceBattle.Auxiliary;
namespace SpaceBattle.Lib.Test
{
    public class VectorTest
    {
        [Fact]
        public void TestAreSameSize()
        {
            var first = new Vector(8, 10, 1, 3, 7);
            var second = new Vector(9, 3, 2, 2, 4);
            var third = new Vector(11, 22, 33);
            Assert.True(Vector.AreSameSize(first, second));
            Assert.False(Vector.AreSameSize(second, third));
        }

        [Fact]
        public void TestSum()
        {
            var first = new Vector(1, 2, 3, 5);
            var second = new Vector(2, 4, 6, 10);
            var third = new Vector(3, 6, 9, 15);
            var fourth = new Vector(11, 22, 33);
            Assert.True(third == first + second);
            Assert.True(third == second + first);
            Assert.Throws<ArgumentException>(() => first + fourth);
        }
        [Fact]
        public void TestIndex()
        {
            var first = new Vector(1, 2, 3, 5);
            Assert.Equal(2, first[1]);
        }
        [Fact]
        public void TestDiff()
        {
            var first = new Vector(1, 2, 3, 5);
            var second = new Vector(2, 4, 6, 10);
            var third = new Vector(3, 6, 9, 15);
            var fourth = new Vector(11, 22, 33);
            Assert.True(first == third - second);
            Assert.True(second == third - first);
            Assert.Throws<ArgumentException>(() => third - fourth);
        }
        [Fact]
        public void MultiplicInt()
        {
            var first = new Vector(1, 2, 3, 4);
            var second = new Vector(2, 4, 6, 8);
            first.MultiplicInt(2);
            Assert.True(second == first);
        }
        [Fact]
        public void TestGetHashCode()
        {
            var first = new Vector(1, 0);
            Assert.IsType<int>(first.GetHashCode());
        }
        [Fact]
        public void TestEquals()
        {
            var a = new Vector(9, 0, 8);
            double b = 3.14;
            var c = new Vector(2, 1, 3);
            Assert.False(a.Equals(b));
            Assert.True(a.Equals(c));
        }
        [Fact]
        public void TestEquality()
        {
            var first = new Vector(1, 1, 1);
            var second = new Vector(1, 1, 1);
            var third = new Vector(1, 2, 1);
            var fourth = new Vector(1, 1);
            Assert.True(first == second);
            Assert.False(first == third);
            Assert.False(second == fourth);
        }

        [Fact]
        public void TestNoEquality()
        {
            var first = new Vector(1, 1);
            var second = new Vector(1, 2);
            var third = new Vector(1, 2, 6);
            var fourth = new Vector(1, 1);
            Assert.True(first != second);
            Assert.True(first != third);
            Assert.False(first != fourth);
        }
    }
}
