using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.gRPC.Services
{
    public class SendCommandByThreadIDStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", args[0]);
            var sendCommand = new SendCommand(sender, (ICommand)args[1]);
            return sendCommand;
        }
    }
}

