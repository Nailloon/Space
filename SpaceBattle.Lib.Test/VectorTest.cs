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
            Assert.True(third == (first + second));
            Assert.True(third == Vector.Summa(second, first));
            Assert.Throws<ArgumentException>(() => Vector.Summa(first, fourth));
        }
    }
}
