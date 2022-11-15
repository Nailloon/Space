using SpaceBattle.Auxiliary;

namespace Tests.TestFraction
{
    public class TestFraction
    {

        [Fact]
        public void TestSumma()
        {
            var a = new Fraction(1, 5);
            var b = new Fraction(4, 5);
            var c = new Fraction(2, 3);
            Assert.True(Fraction.AreEquals(new Fraction(13, 15), Fraction.Summa(a, c)));
            Assert.True(Fraction.AreEquals(new Fraction(1, 1), Fraction.Summa(a, b)));
            Assert.False(Fraction.AreEquals(new Fraction(2, 3), Fraction.Summa(b, c)));
        }

        [Fact]
        public void TestSubstraction()
        {
            var a = new Fraction(-5, 10);
            var b = new Fraction(1, 6);
            var c = new Fraction(-8, 6);
            Assert.True(Fraction.AreEquals(new Fraction(-2, 3), Fraction.Subtraction(a, b)));
            Assert.False(Fraction.AreEquals(new Fraction(-7, 6), Fraction.Subtraction(b, c)));
        }

        [Fact]
        public void TestMultInt()
        {
            var a = new Fraction(1, -2);
            Assert.True(Fraction.AreEquals(new Fraction(-2, 1), Fraction.MultInt(4, a)));
            Assert.False(Fraction.AreEquals(new Fraction(5, -2), Fraction.MultInt(-5, a)));
        }

        [Fact]
        public void TestForm()
        {
            var a = new Fraction(2, 4);
            var b = new Fraction(3, 4);
            Assert.True(Fraction.AreEquals(new Fraction(1, 2), Fraction.Form(a)));
            Assert.True(Fraction.AreEquals(new Fraction(3, 4), Fraction.Form(b)));
            Assert.False(Fraction.AreEquals(new Fraction(2, 4), Fraction.Form(a)));
        }
    }
}
