using SpaceBattle.Interfaces;
using System.Collections.Concurrent;

namespace SpaceBattle.Server
{
    public class ReceiverAdapter : IReceiver
    {
        BlockingCollection<ICommand> commands;
        public ReceiverAdapter(BlockingCollection<ICommand> q)
        {
            this.commands = q;
        }
        public ICommand Receive()
        {
            return commands.Take();
        }
        public bool IsEmpty()
        {
            return commands.Count() == 0;
        }
    }
}
