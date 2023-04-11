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
    public class CreateSenderToThreadStrategy: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            return new SenderAdapter((BlockingCollection<ICommand>)args[0]);
        }
    }
}
