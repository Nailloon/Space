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
            if (args.Length > 1)
            {
                Action act1 = (Action)args[1];
                var softStopCommand = IoC.Resolve<ICommand>("CommandForSoftStopStrategy", MT, act1);
                return IoC.Resolve<ICommand>("SendCommand", sender, softStopCommand);
            }
            else
            {
                var softStopCommand = IoC.Resolve<ICommand>("CommandForSoftStopStrategy", MT);
                return IoC.Resolve<ICommand>("SendCommand", sender, softStopCommand);
            }
        }
    }
}
