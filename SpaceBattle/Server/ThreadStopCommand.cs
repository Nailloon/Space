using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.Server
{
    public class ThreadStopCommand: ICommand
    {
        MyThread stoppingThread;
        public ThreadStopCommand(MyThread stoppingThread)
        {
            this.stoppingThread = stoppingThread;
        }
        public void Execute()
        {
            if (Thread.CurrentThread.Equals(stoppingThread))
            {
                stoppingThread.Stop();
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
