using Hwdtech;
using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.Auxiliary;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test
{
    public class StartMoveCommandTest
    {
        public StartMoveCommandTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            var mockCommand = new Mock<Interfaces.ICommand>();
            mockCommand.Setup(_command => _command.Execute());
            var regStrategy = new Mock<IStrategy>();
            regStrategy.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(mockCommand.Object);

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Сomprehensive.SetProperty", (object[] args) => regStrategy.Object.StartStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapt.Move", (object[] args) => regStrategy.Object.StartStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Queue.Push", (object[] args) => regStrategy.Object.StartStrategy(args)).Execute();
        }

        [Fact]
        public void PositiveTest_StartMoveCommand()
        {
            var moveCommandStartable = new Mock<IMoveCommandStartable>();
            moveCommandStartable.SetupGet(x => x.Uobj).Returns(new Mock<IUObject>().Object).Verifiable();
            moveCommandStartable.SetupGet(x => x.action).Returns(new Dictionary<string, object>() { { "Velocity", new Vector(It.IsAny<int>(), It.IsAny<int>()) } }).Verifiable();
            Interfaces.ICommand SMC = new StartMoveCommand(moveCommandStartable.Object);
            SMC.Execute();
            moveCommandStartable.Verify();
        }
        [Fact]
        public void NegativeTest_StartMoveCommand_UnableToGetUObject()
        {
            var moveCommandStartable = new Mock<IMoveCommandStartable>();
            moveCommandStartable.SetupGet(x => x.Uobj).Throws<Exception>().Verifiable();
            moveCommandStartable.SetupGet(x => x.action).Returns(new Dictionary<string, object>() { { "Velocity", new Vector(It.IsAny<int>(), It.IsAny<int>()) } }).Verifiable();
            Interfaces.ICommand SMC = new StartMoveCommand(moveCommandStartable.Object);
            Assert.Throws<Exception>(() => SMC.Execute());
        }
        [Fact]
        public void NegativeTest_StartMoveCommand_UnableToGetVelocity()
        {
            var moveCommandStartable = new Mock<IMoveCommandStartable>();
            moveCommandStartable.SetupGet(x => x.Uobj).Returns(new Mock<IUObject>().Object).Verifiable();
            moveCommandStartable.SetupGet(x => x.action).Throws<Exception>().Verifiable();
            Interfaces.ICommand SMC = new StartMoveCommand(moveCommandStartable.Object);
            Assert.Throws<Exception>(() => SMC.Execute());
        }
        [Fact]
        public void PositiveTest_IQueue()
        {
            var moveCommandStartable = new Mock<IMoveCommandStartable>();
            Interfaces.ICommand SMC = new StartMoveCommand(moveCommandStartable.Object);
            var queue = new Mock<IQueue<Interfaces.ICommand>>();
            queue.Setup(x => x.Push(SMC)).Verifiable();
            queue.Object.Push(SMC);
            queue.Verify();
        }
    }
}
