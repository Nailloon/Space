using SpaceBattle.Interfaces;
namespace SpaceBattle.MacroCommand

{
    public class MacroCommands : Interfaces.ICommand
    {
        IEnumerable<Interfaces.ICommand> commands;
        public MacroCommands(IEnumerable<Interfaces.ICommand> commands)
        {
            this.commands = commands;
        }
        public void Execute()
        {
            foreach (Interfaces.ICommand command in commands)
            {
                command.Execute();
            }
        }
    }
}
