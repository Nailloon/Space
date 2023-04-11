using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.ServerStrategies
{
    public class CreateAPair: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            BlockingCollection<ICommand> que = new BlockingCollection<ICommand>(100);
            var MT = IoC.Resolve<MyThread>("CreateAndStartThread", que, (Action)args[0]);
            var Sender = new SenderAdapter(que);
            var retList = new List<object>();
            retList.Add(MT);
            retList.Add(Sender);
            return retList;
        }
    }
}
