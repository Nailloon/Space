using ICommand = SpaceBattle.Interfaces.ICommand;
using Hwdtech;

namespace SpaceBattle.SuperGameCommand
{
    public class MacroGameCommand: ICommand
    {
        string id;
        GameCommand gameCommand;
        object scope;
        public MacroGameCommand(string id, object scope, Queue<ICommand> queue)
        {
            this.id = id;
            this.gameCommand = new GameCommand(id, queue);
            this.scope = scope;
        }
        public void Execute()
        {
            var initialScope = IoC.Resolve<object>("ThreadScope.Current", id);
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", this.scope).Execute();

            gameCommand.Execute();

            var threadID = IoC.Resolve<string>("Storage.GetThreadByGameID", id);

            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", initialScope).Execute();
        }
    }
}
