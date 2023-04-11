using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.ServerStrategies
{
    public class CreateAndStartThreadStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var commands = (BlockingCollection<ICommand>)args[0];
            commands.Add(new ActionCommand((Action)args[1]));
            IReceiver queue = new ReceiverAdapter(commands);
            var MT = new MyThread((IReceiver)args[0]);
            return MT;
        }
    }
}
