using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System.Collections.Concurrent;

namespace SpaceBattle.ServerStrategies
{
    public class CreateAndStartThreadStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var senderDict = IoC.Resolve<ConcurrentDictionary<string, ISender>>("ThreadIDSenderMapping");
            senderDict.TryAdd((string)args[0], (ISender)args[1]);
            var senderOrderDict = IoC.Resolve<ConcurrentDictionary<string, ISender>>("ThreadIDOrdersSenderMapping");
            senderOrderDict.TryAdd((string)args[0], (ISender)args[3]);
            var MT = new MyThread((IReceiver)args[2], (IReceiver)args[4]);
            MT.Execute();
            var threadDict = IoC.Resolve<ConcurrentDictionary<string, MyThread>>("ThreadIDMyThreadMapping");
            threadDict.TryAdd((string)args[0], MT);
            return MT;
        }
    }
}
