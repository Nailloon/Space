using SpaceBattle.Interfaces;
using System.Collections.Concurrent;

namespace SpaceBattle.Server
{
    public class CreateAndStartStrategy: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var id = (string)args[0];
            BlockingCollection<SpaceBattle.Interfaces.ICommand > commands = new BlockingCollection<SpaceBattle.Interfaces.ICommand>(100);
            ReceiverAdapter queue = new ReceiverAdapter(commands);
            var MT = new MyThread(queue);
            if (args.Length > 1)
            {
                var action = new ActionCommand((Action)args[1]);
                MT.UpdateBehavior(action);
            }  
            return MT;
        }
    }
}
