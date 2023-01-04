using Hwdtech;
using SpaceBattle.Interfaces;

namespace SpaceBattle.MacroCommand
{
    public class MacroCommandFactoryStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var name = args[0];
            IEnumerable<string> names = IoC.Resolve<IEnumerable<string>>("Config.MacroCommand." + name);
            IEnumerable<Interfaces.ICommand> commands = new List<Interfaces.ICommand>();
            foreach (string command in names)
            {
                commands = commands.Concat(new[] { IoC.Resolve<Interfaces.ICommand>("Concat.Commands", command, args[1]) });
            }
            return IoC.Resolve<Interfaces.ICommand>("SimpleMacroCommand", commands);
        }
    }
}
