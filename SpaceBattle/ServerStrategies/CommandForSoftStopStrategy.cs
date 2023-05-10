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
            if (args.Length > 1)
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
                        act1();
                    }
                });
                return new UpdateBehaviorCommand((MyThread)args[0], act);
            }
            else
            {
                Action act = new Action(() =>
                {
                    if (!MT.QueueIsEmpty())
                    {
                        MT.HandleCommand();
                    }
                    else
                    {
                        new ThreadStopCommand(MT).Execute();
                    }
                });
                return new UpdateBehaviorCommand((MyThread)args[0], act);
            }
        }
    }
}
