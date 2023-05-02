using Hwdtech;
using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.MacroCommand;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test
{
    public class MacroCommandTest
    {
        public MacroCommandTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }
        [Fact]
        public void PositiveMacroCommandTest()
        {
            var command1 = new Mock<Interfaces.ICommand>();
            var command2 = new Mock<Interfaces.ICommand>();
            command1.Setup(_command => _command.Execute()).Verifiable();
            command2.Setup(_command => _command.Execute()).Verifiable();
            var commands = new List<Interfaces.ICommand> { command1.Object, command2.Object };
            MacroCommands macroCommand = new MacroCommands(commands);
            macroCommand.Execute();
        }
        [Fact]
        public void NegativeMacroCommandTest()
        {
            var command1 = new Mock<Interfaces.ICommand>();
            var command2 = new Mock<Interfaces.ICommand>();
            command1.Setup(_command => _command.Execute()).Verifiable();
            command2.Setup(_command => _command.Execute()).Throws<Exception>().Verifiable();
            var commands = new List<Interfaces.ICommand> { command1.Object, command2.Object };
            MacroCommands macroCommand = new MacroCommands(commands);
            Assert.Throws<Exception>(() => macroCommand.Execute());
        }
    }
}
