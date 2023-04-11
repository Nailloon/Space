using SpaceBattle.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Server
{
    public class SenderAdapter : ISender
    {
        BlockingCollection<SpaceBattle.Interfaces.ICommand> queue;
        public SenderAdapter(BlockingCollection<SpaceBattle.Interfaces.ICommand> queue)
        {
            this.queue = queue;
        }
        public void Send(ICommand command)
        {
            queue.Add(command);
        }
    }
}
