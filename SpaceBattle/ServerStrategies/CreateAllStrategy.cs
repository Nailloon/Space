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
            var sender = new SenderAdapter(que);
            var receiveradapter = IoC.Resolve<ReceiverAdapter>("CreateReceiverAdapter", que);
            if (args.Length>1)
            {
                var MT = IoC.Resolve<MyThread>("CreateAndStartThread", (string)args[0], sender, receiveradapter, args[1]);
                return MT;
            }
            else
            {
                var MT = IoC.Resolve<MyThread>("CreateAndStartThread", (string)args[0], sender, receiveradapter);
                return MT;
            }
        }
    }
}
