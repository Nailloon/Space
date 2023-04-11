using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using Hwdtech;
using System.Collections.Concurrent;
using System.Reflection;

namespace SpaceBattle.ServerStrategies
{
    public class CreateNewDictionariesStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            Dictionary<string, object> ThreadDictionary = new Dictionary<string, object>();
            Dictionary<string, object> SenderDictionary = new Dictionary<string, object>();
            var listOfThreadAndSenderAdapter = IoC.Resolve<List<object>>("CreatePair", (Action)args[1]);
            ThreadDictionary.Add((string)args[0], listOfThreadAndSenderAdapter[0]);
            SenderDictionary.Add((string)args[0], listOfThreadAndSenderAdapter[1]);
            var resultList = new List<object>();
            resultList.Add(ThreadDictionary);
            resultList.Add(SenderDictionary);
            return resultList;
        }
    }
}
