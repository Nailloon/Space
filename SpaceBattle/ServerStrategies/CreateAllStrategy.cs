using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System.Collections.Concurrent;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.ServerStrategies
{
    public class CreateAllStrategy: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            Action? action1 = (Action)args[1];
            BlockingCollection<ICommand> que = new BlockingCollection<ICommand>(100);
            var receiveradapter = IoC.Resolve<ReceiverAdapter>("CreateReceiverAdapter", que, action1);
            var sender = new SenderAdapter(que);
            var MT = IoC.Resolve<MyThread>("CreateAndStartThread", (string)args[0], sender, receiveradapter);
            return MT;
        }
    }
}
