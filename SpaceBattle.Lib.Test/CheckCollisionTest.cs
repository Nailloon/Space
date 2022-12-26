using Hwdtech;
using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.Collision;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test
{
    public class CheckCollisionTest
    {
        public CheckCollisionTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            var mockCommand = new Mock<Interfaces.ICommand>();
            mockCommand.Setup(_command => _command.Execute());
            var regStrategy = new Mock<IStrategy>();
            regStrategy.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(mockCommand.Object);
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Calculate.Delta", (object[] args) => regStrategy.Object.StartStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CollisionDecisionTree", (object[] args) => regStrategy.Object.StartStrategy(args)).Execute();
        }
        [Fact]
        public void NoCollision_and_NoException()
        {
            var DeltaStrategy = new Mock<IStrategy>();
            DeltaStrategy.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(new List<int>()).Verifiable();
            var DecisionStrategy = new Mock<IStrategy>();
            DecisionStrategy.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(false).Verifiable();
            var UnicObject1 = new Mock<IUObject>();
            var UnicObject2 = new Mock<IUObject>();
            CheckCollision collision = new CheckCollision(UnicObject1.Object, UnicObject2.Object);
            collision.Execute();
            DecisionStrategy.Verify();
            DeltaStrategy.Verify();
        }

    }
}
