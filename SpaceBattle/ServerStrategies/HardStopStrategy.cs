using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.ServerStrategies
{
    public class HardStopStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var id = args[0];
            var MT = IoC.Resolve<MyThread>("ServerThreadGetByID", id);
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", id);
            var hardStopCommand = new ThreadStopCommand(MT);
            if (args.Length > 1)
            {
                Action act = (Action)args[1];
                var macroHardStopCommand = IoC.Resolve<ICommand>("MacroCommandForHardStopStrategy", hardStopCommand, act);
                return IoC.Resolve<ICommand>("SendCommand", sender, macroHardStopCommand);
            }
            else
            {
                return IoC.Resolve<ICommand>("SendCommand", sender, hardStopCommand);
            }
        }
    }
}
