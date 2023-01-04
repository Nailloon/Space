using Hwdtech;
using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.MacroCommand;
using Hwdtech.Ioc;
using SpaceBattle.Auxiliary;

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
            var commands = new Mock<IEnumerable<Interfaces.ICommand>>();
            MacroCommands macroCommand = new MacroCommands(commands.Object);
            macroCommand.Execute();
        }
    }
}
