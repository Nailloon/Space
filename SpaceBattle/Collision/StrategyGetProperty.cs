using SpaceBattle.Interfaces;

namespace SpaceBattle.Collision
{
    public class StrategyGetProperty : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            IUObject uobject = (IUObject)args[0];
            string propertyName = (string)args[1];
            return uobject.get_property(propertyName);
        }
    }
}
