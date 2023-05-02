using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.MacroCommand;

namespace SpaceBattle.Lib.Test
{
    public class MacroCommandFactoryStrategyTest
    {
        public MacroCommandFactoryStrategyTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }
        [Fact]
        public void PositiveMacroCommandFactoryStrategyTest()
        {
            var UnicObject1 = new Mock<IUObject>();
            var command1 = new Mock<Interfaces.ICommand>();
            var command2 = new Mock<Interfaces.ICommand>();
            var StrategyMacroCommandFactory = new MacroCommandFactoryStrategy();
            var regStrategy = new Mock<IStrategy>();
            regStrategy.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(new List<string> { "MvCommand1", "MvCommand2"}).Verifiable();
            var regStrategy1 = new Mock<IStrategy>();
            regStrategy1.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(command1.Object).Verifiable();
            var regStrategy2 = new Mock<IStrategy>();
            regStrategy2.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(command2.Object).Verifiable();
            var commands = new List<Interfaces.ICommand> { command1.Object, command2.Object };
            MacroCommands macroCommand = new MacroCommands(commands);
            var itog = new Mock<IStrategy>();
            itog.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object>())).Returns(macroCommand).Verifiable();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Config.MacroCommand.Move", (object[] args) => regStrategy.Object.StartStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "MvCommand1", (object[] args) => regStrategy1.Object.StartStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "MvCommand2", (object[] args) => regStrategy2.Object.StartStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SimpleMacroCommand", (object[] args) => itog.Object.StartStrategy(args)).Execute();


            StrategyMacroCommandFactory.StartStrategy("Move", UnicObject1.Object);
            regStrategy.Verify();
            regStrategy1.Verify();
            regStrategy2.Verify();
            itog.Verify();
        }
    }
}
