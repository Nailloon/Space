using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.Move;
using SpaceBattle.Auxiliary;

namespace SpaceBattle.Lib.Test
{
    public class MoveTest
    {
        [Fact]
        public void TestPositiveMove()
        {
            //PRE
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupProperty<Vector>(m => m.Position, new Vector(12, 5));
            movable.SetupGet<Vector>(m => m.Velocity).Returns(new Vector(-7, 3));
            ICommand MC = new MoveCommand(movable.Object);
            //ACTION
            MC.Execute();
            //POST
            Assert.True(new Vector(5, 8) == movable.Object.Position);
        }
        [Fact]
        public void GetPositionExpection()
        {
            //PRE
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupGet<Vector>(m => m.Position).Throws<Exception>();
            ICommand MC = new MoveCommand(movable.Object);
            //POST
            Assert.Throws<Exception>(() => MC.Execute());
        }
        [Fact]
        public void SetPositionExpection()
        {
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupGet<Vector>(m => m.Position).Returns(new Vector(10, 2));
            movable.SetupGet<Vector>(m => m.Velocity).Returns(new Vector(-5, 1));
            movable.SetupSet<Vector>(m => m.Position = It.IsAny<Vector>()).Throws<Exception>();
            ICommand MC = new MoveCommand(movable.Object);
            Assert.Throws<Exception>(() => MC.Execute());
        }
        [Fact]
        public void GetVelocityExpection()
        {
            Mock<IMovable> movable = new Mock<IMovable>();
            movable.SetupGet<Vector>(m => m.Velocity).Throws<Exception>();
            ICommand MC = new MoveCommand(movable.Object);
            Assert.Throws<Exception>(() => MC.Execute());
        }
    }
}
