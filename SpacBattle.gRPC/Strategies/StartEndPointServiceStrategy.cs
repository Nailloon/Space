namespace SpaceBattleGrpc.Strategies;

using SpaceBattle.Interfaces;

public class StartEndPointServiceStrategy : IStrategy
{
    public object StartStrategy(params object[] argv)
    {
        var builderArgs = (string[])argv[0];
        return new StartEndPointServiceCommand(args: builderArgs);
    }
}
