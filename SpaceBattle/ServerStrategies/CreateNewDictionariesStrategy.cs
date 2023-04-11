using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using Hwdtech;
using System.Collections.Concurrent;

namespace SpaceBattle.ServerStrategies
{
    public class CreateNewDictionariesStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            Dictionary<string, object> ThreadDictionary = new Dictionary<string, object>();
            Dictionary<string, object> SenderDictionary = new Dictionary<string, object>();
            var que = IoC.Resolve<BlockingCollection<SpaceBattle.Interfaces.ICommand>>("CreateQueue");
            var MT = IoC.Resolve<MyThread>("CreateAndStartThread", que, (Action)args[1]);
            ThreadDictionary.Add((string)args[0], MT);
            var Sender = IoC.Resolve<SenderAdapter>("CreateSenderToThread", que);
            SenderDictionary.Add((string)args[0], Sender);
            return (ThreadDictionary, SenderDictionary);
        }
    }
}
