using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System.Collections.Concurrent;

namespace SpaceBattle.ServerStrategies
{
    public class CreateAndStartStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            BlockingCollection<ICommand> commands = new BlockingCollection<ICommand>(100);
            commands.Add(new ActionCommand((Action)args[0]));
            IReceiver queue = new ReceiverAdapter(commands);
            var MT = new MyThread(queue);
            return MT;
        }
    }
}
