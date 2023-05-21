using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.Server
{
    public class UpdateBehaviorCommand: ICommand
    {
        MyThread updateBehaviorThread;
        Action action;
        public UpdateBehaviorCommand(MyThread updateBehaviorThread, Action action)
        {
            this.updateBehaviorThread = updateBehaviorThread;
            this.action = action;
        }
        public void Execute()
        {
            if (updateBehaviorThread.Equals(Thread.CurrentThread))
            {
                updateBehaviorThread.UpdateBehavior(action);
            }
            else
            {
                throw new Exception();
            } 
        }
    }
}
