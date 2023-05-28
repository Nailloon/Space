using System.Diagnostics;
using Hwdtech;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.SuperGameCommand
{
    public class GameCommand: ICommand
    {
        Queue<ICommand> queue = new Queue<ICommand>();
        string gameId;
        Stopwatch stopwatch = new Stopwatch();

        public GameCommand(string gameId)
        {
            this.gameId = gameId;
        }
        public void Execute()
        {
            var quantumOfTime = IoC.Resolve<TimeSpan>("QuantumForGame");
            stopwatch.Restart();
            while(stopwatch.Elapsed < quantumOfTime)
            {
                var retrieved = queue.TryDequeue(out var command);
                try
                {
                    if(retrieved==true)
                    command!.Execute();
                }
                catch(Exception exception)
                {
                    IoC.Resolve<ICommand>("GameHandleException", exception, command!);
                }
            }
            stopwatch.Stop();
        }
    }
}
