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
            if (stoppingThread.Equals(Thread.CurrentThread))
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
