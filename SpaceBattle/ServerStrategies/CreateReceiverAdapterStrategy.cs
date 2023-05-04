using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System.Collections.Concurrent;

namespace SpaceBattle.ServerStrategies
{
    public class CreateReceiverAdapterStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var commands = (BlockingCollection<ICommand>)args[0];
            IReceiver queue = new ReceiverAdapter(commands);
            return queue;
        }
    }
}
