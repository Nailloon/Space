using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using Hwdtech;

namespace SpaceBattle.ServerStrategies
{
    public class AddNewThreadStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            Dictionary<string, object> copy = (Dictionary<string, object>)args[0];
            copy.Add((string)args[1], IoC.Resolve<MyThread>("CreateAndStartThread", (Action)args[2]));
            return copy;
        }
    }
}
