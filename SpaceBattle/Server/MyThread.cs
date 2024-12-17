using SpaceBattle.Interfaces;
using Hwdtech;

namespace SpaceBattle.Server
{
    public class MyThread
    {
        public MyThread(IReceiver queue, IReceiver orderQueue)
        {
            this.queue = queue;
            this.orderQueue = orderQueue;
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
        internal bool stop = false;
        private IReceiver queue;
        private IReceiver orderQueue;
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
            if (!orderQueue.IsEmpty()){
                SpaceBattle.Interfaces.ICommand order = orderQueue.Receive();
                tryExecute(order);
            }
            SpaceBattle.Interfaces.ICommand cmd = queue.Receive();
            tryExecute(cmd);
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
        internal void tryExecute(SpaceBattle.Interfaces.ICommand command)
        {
            try
            {
                command.Execute();
            }
            catch(Exception e)
            {
                var exceptionCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HandleException", e, command);
                exceptionCommand.Execute();
            }
        }
    }
}
