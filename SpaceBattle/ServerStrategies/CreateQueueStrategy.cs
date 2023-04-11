using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.ServerStrategies
{
    public class CreateQueueStrategy: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            BlockingCollection<ICommand> commands = new BlockingCollection<ICommand>(100);
            return commands;
        }
    }
}
