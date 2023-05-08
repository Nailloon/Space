using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.ServerStrategies
{
    public class SoftStopStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var id = args[0];
            var MT = IoC.Resolve<MyThread>("ServerThreadGetByID", id);
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", id);
            var hardStopCommand = new ThreadStopCommand(MT);
            if (args.Length > 1)
            {
                Action? act = (Action?)args[1];
                var softStopCommand = IoC.Resolve<ICommand>("MacroCommandForSoftStopStrategy", hardStopCommand, act);
                return IoC.Resolve<ICommand>("SendCommand", sender, softStopCommand);
            }
            else
            {
                return IoC.Resolve<ICommand>("SendCommand", sender, hardStopCommand);
            }
        }
    }
}
