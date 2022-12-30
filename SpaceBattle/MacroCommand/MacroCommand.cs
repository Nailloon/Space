using SpaceBattle.Interfaces;
namespace SpaceBattle.MacroCommand

{
    public class MacroCommand : Interfaces.ICommand
    {
        IEnumerable<Interfaces.ICommand> commands;
        public MacroCommand(IEnumerable<Interfaces.ICommand> commands)
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
