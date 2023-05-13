using SpaceBattle.Interfaces;
using SpaceBattle.MacroCommand;
using SpaceBattle.Server;

namespace SpaceBattle.ServerStrategies
{
    public class CommandForSoftStopStrategy: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var MT = (MyThread)args[0];
            {
                Action act1 = (Action)args[1];
                Action act = new Action(() =>
                {
                    if (!MT.QueueIsEmpty())
                    {
                        MT.HandleCommand();
                    }
                    else
                    {
                        new ThreadStopCommand(MT).Execute();
                        if (args.Length > 1)
                            act1();
                    }
                });
                return new UpdateBehaviorCommand((MyThread)args[0], act);
            }
        }
    }
}
