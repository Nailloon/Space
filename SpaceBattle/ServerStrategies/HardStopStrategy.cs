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
            Action? act = (Action?)args[1];
            var MT = IoC.Resolve<MyThread>("ServerThreadGetByID", id);
            var sender = IoC.Resolve<SenderAdapter>("SenderAdapterGetByID", id);
            var hardStopCommand = new ThreadStopCommand(MT, act);
            return IoC.Resolve<ICommand>("SendCommand", sender, hardStopCommand);
        }
    }
}
