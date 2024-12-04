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
            BlockingCollection<ICommand> que = new BlockingCollection<ICommand>(100);
            BlockingCollection<ICommand> ords = new BlockingCollection<ICommand>(100);
            var sender = new SenderAdapter(que);
            if (args.Length > 1)
            {
                sender.Send(new ActionCommand((Action)args[1]));
            }
            var sender2 = new SenderAdapter(ords);
            sender2.Send(new ActionCommand(()=>{}));
            var receiveradapter = IoC.Resolve<ReceiverAdapter>("CreateReceiverAdapter", que);
            var receiverAdapter2 = IoC.Resolve<ReceiverAdapter>("CreateReceiverAdapter", ords);
            var MT = IoC.Resolve<MyThread>("CreateAndStartThread", (string)args[0], sender, receiveradapter, sender2, receiverAdapter2);
            return MT;
        }
    }
}
