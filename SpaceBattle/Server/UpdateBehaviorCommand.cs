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
            updateBehaviorThread.UpdateBehavior(action);
        }
    }
}
