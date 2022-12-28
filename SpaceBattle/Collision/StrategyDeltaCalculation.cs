using Hwdtech;
using SpaceBattle.Auxiliary;
using SpaceBattle.Interfaces;

namespace SpaceBattle.Collision
{
    public class StrategyDeltaCalculation : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var uObj1 = (IUObject)args[0];
            var uObj2 = (IUObject)args[1];
            List<string> names = new List<string> { "Coords", "Velocity" };
            List<int> useful = new List<int>();
            //foreach ()string name in names
            for (int i = 0; i < 2; i++)
            {
                var uObj1Vec = IoC.Resolve<Vector>("UObject.GetProperty", uObj1, names[i]);
                var uObj2Vec = IoC.Resolve<Vector>("UObject.GetProperty", uObj2, names[i]);
                useful.Add(uObj1Vec[0] - uObj2Vec[0]);
                useful.Add(uObj1Vec[1] - uObj2Vec[1]);
            }
            return new Vector(useful);
        }
    }
}
