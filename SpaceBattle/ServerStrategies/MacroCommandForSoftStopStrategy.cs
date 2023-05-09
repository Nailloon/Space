using SpaceBattle.Interfaces;
using SpaceBattle.MacroCommand;
using SpaceBattle.Server;

namespace SpaceBattle.ServerStrategies
{
    public class MacroCommandForSoftStopStrategy: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            IEnumerable<Interfaces.ICommand> commands = new List<Interfaces.ICommand>();
            commands.Append(new UpdateBehaviorCommand((MyThread)args[1], (Action)args[2]));
            commands.Append(args[0]);
            return new MacroCommands(commands);
        }
    }
}
