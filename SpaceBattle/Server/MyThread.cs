using SpaceBattle.Interfaces;
using Hwdtech;

namespace SpaceBattle.Server
{
    public class MyThread
    {
        public MyThread(IReceiver queue)
        {
            this.queue = queue;
            strategy = new Action(() =>
            {
                HandleCommand();
            });
            this.thread = new Thread(() =>
            {
                while (!stop)
                {
                    strategy.Invoke();
                }
            });
        }
        bool stop = false;
        private IReceiver queue;
        private Thread thread;
        private Action strategy;
        public void Stop()
        {
            stop = true;
        }
        public void Execute()
        {
            thread.Start();
        }
        internal void HandleCommand()
        {
            SpaceBattle.Interfaces.ICommand cmd = this.queue.Receive();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                var exceptionCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HandleException", e, cmd);
                exceptionCommand.Execute();
            }
        }
        public void UpdateBehavior(Action newBeh)
        {
            strategy = newBeh;
        }
        public bool GetStop()
        {
            return this.stop;
        }
        public bool QueueIsEmpty()
        {
            return queue.IsEmpty();
        }
        public bool Equals(Thread thread)
        {
            return this.thread == thread;
        }
    }
}
