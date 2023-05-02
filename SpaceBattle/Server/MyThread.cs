using SpaceBattle.Interfaces;
using Hwdtech;

namespace SpaceBattle.Server
{
    public class MyThread
    {
        public MyThread(IReceiver queue)
        {
            this.queue = queue;
            strategy = new ActionCommand(() =>
            {
                HandleCommand();
            });
            this.thread = new Thread(() =>
            {
                while (!stop)
                {
                    strategy.Execute();
                }
            });
        }
        bool stop = false;
        private IReceiver queue;
        private Thread thread;
        private ActionCommand strategy;
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
            }catch(Exception e)
            {
                IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HandleException", e, cmd);
            }
        }
        public void UpdateBehavior(ActionCommand newBeh)
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
    }
}
