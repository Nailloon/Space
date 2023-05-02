using SpaceBattle.Interfaces;
using System.Collections.Concurrent;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.Server
{
    public class SenderAdapter : ISender
    {
        BlockingCollection<ICommand> queue;
        public SenderAdapter(BlockingCollection<ICommand> queue)
        {
            this.queue = queue;
        }
        public void Send(ICommand command)
        {
            queue.Add(command);
        }
    }
}
