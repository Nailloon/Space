using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.Server
{
    public class HardStopCommand: ICommand
    {
        private Action action1;
        private Action action2;
        private string id;
        public HardStopCommand(params object[] args)
        {
            this.id = (String)args[0];
            Action action1 = (Action)args[1];
            this.action1 = action1;
            Action? action2 = (Action?)args[2];
            this.action2 = action2;
        }
        public void Execute()
        {
            action2.Invoke();
            action1();
        }
    }
}
