using ICommand = SpaceBattle.Interfaces.ICommand;
using Hwdtech;

namespace SpaceBattle.GameCommand
{
    public class RepeatGameCommand: ICommand
    {
        string id;
        GameCommand gameCommand;
        object scope;
        public RepeatGameCommand(string id, object scope)
        {
            this.id = id;
            gameCommand = new GameCommand(id);
            this.scope = scope;
        }
        public void Execute()
        {
            var initialScope = IoC.Resolve<object>("Scopes.Current");
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", this.scope).Execute();

            this.gameCommand.Execute();
            var threadID = IoC.Resolve<string>("Storage.GetThreadByGameID", this.id);
            IoC.Resolve<ICommand>("SendCommandByThreadIDStrategy", threadID, this).Execute();

            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", initialScope).Execute();
        }
    }
}
