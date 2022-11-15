using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.Rotate;
using SpaceBattle.Auxiliary;

namespace SpaceBattle.Lib.Test
{
    public class RotateTest
    {
        [Fact]
        public void TestPositiveRotate()
        {
            Mock<IRotate> rotateble = new Mock<IRotate>();
            rotateble.SetupProperty<Fraction>(r => r.Angle, new Fraction(1, 2));
            rotateble.SetupGet<Fraction>(r => r.AngleVelocity).Returns(new Fraction(6, 3));
            ICommand RC = new RotateCommand(rotateble.Object);
            //ACTION
            RC.Execute();
            //POST
            Assert.True(Fraction.AreEquals(new Fraction(5, 2), rotateble.Object.Angle));
        }
        [Fact]
        public void GetAngleExpection()
        {
            //PRE
            Mock<IRotate> rotateble = new Mock<IRotate>();
            rotateble.SetupGet<Fraction>(r => r.Angle).Throws<Exception>();
            ICommand RC = new RotateCommand(rotateble.Object);
            //POST
            Assert.Throws<Exception>(() => RC.Execute());
        }
        [Fact]
        public void SetAngleExpection()
        {
            Mock<IRotate> rotateble = new Mock<IRotate>();
            rotateble.SetupGet<Fraction>(r => r.Angle).Returns(new Fraction(10, 2));
            rotateble.SetupGet<Fraction>(r => r.AngleVelocity).Returns(new Fraction(-5, 1));
            rotateble.SetupSet<Fraction>(r => r.Angle = It.IsAny<Fraction>()).Throws<Exception>();
            ICommand RC = new RotateCommand(rotateble.Object);
            Assert.Throws<Exception>(() => RC.Execute());
        }
    }
}
