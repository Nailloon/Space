using Hwdtech;
using SpaceBattle.Interfaces;

namespace SpaceBattle.MacroCommand
{
    public class MacroCommandFactoryStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var name = (string)args[0];
            var uObj = (IUObject)args[1];
            IEnumerable<string> names = IoC.Resolve<IEnumerable<string>>("Config.MacroCommand." + name);
            IEnumerable<Interfaces.ICommand> commands = new List<Interfaces.ICommand>();
            foreach (string command in names)
            {
                commands.Append(IoC.Resolve<Interfaces.ICommand>(command, uObj));
            }
            return IoC.Resolve<Interfaces.ICommand>("SimpleMacroCommand", commands);
        }
    }
}
