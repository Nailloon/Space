namespace SpaceBattleGrpc.Others;

using SpaceBattle.Interfaces;

public class StartEndPointServiceStrategy : IStrategy
{
    public object StartStrategy(params object[] args)
    {
        return new StartEndPointServiceCommand();
    }
}
