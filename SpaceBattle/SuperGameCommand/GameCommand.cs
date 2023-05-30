using System.Diagnostics;
using Hwdtech;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.SuperGameCommand
{
    public class GameCommand: ICommand
    {
        Queue<ICommand> queue;
        string gameId;
        Stopwatch stopwatch = new Stopwatch();

        public GameCommand(string gameId, Queue<ICommand> queue)
        {
            this.gameId = gameId;
            this.queue = queue;
        }
        public void Execute()
        {
            var quantumOfTime = IoC.Resolve<TimeSpan>("QuantumForGame");
            stopwatch.Restart();
            while(stopwatch.Elapsed < quantumOfTime)
            {
                if (queue.Count() == 0)
                    break;
                var retrieved = queue.TryDequeue(out var command);
                try
                {
                    command!.Execute();
                }
                catch (Exception exception)
                {
                    IoC.Resolve<ICommand>("HandleException", exception, command!);
                }
            }
            stopwatch.Stop();
        }
    }
}
