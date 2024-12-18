using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.SuperGameCommand;

namespace SpaceBattle.gRPC.Others
{
    public class DeserializeCommand : Interfaces.ICommand
    {
        private string threadId;
        private string serializedString;

        public DeserializeCommand(string threadId, string serializedGame)
        {
            this.threadId = threadId;
            serializedString = serializedGame;
        }

        public void Execute()
        {
            string[] serializedData = serializedString.Split('|');
            List<string> gameOptions = new();
            Dictionary<string, object> gameObjects = new();
            Queue<Interfaces.ICommand> gameQueue = new();

            foreach(string optionData in serializedData[0].Split(',')){
                if(optionData.Contains("Scope"))
                {
                    gameOptions.Add(optionData);
                }
            }
            
            foreach(string propertyData in serializedData[1].Split(';')){
                if (propertyData.Contains(':'))
                {
                    string key = propertyData.Split(" : ")[0];
                    string stringValue = propertyData.Split(" : ")[1];

                    object objectValue = IoC.Resolve<object>("DeserializeValue", stringValue);

                    gameObjects[key] = objectValue;
                }

            }

            foreach(string commandData in serializedData[2].Split(',')){
                if(commandData.Contains("type"))
                {
                    Interfaces.ICommand deserializedCommand = IoC.Resolve<Interfaces.ICommand>("DeserializeCommand", commandData);

                    gameQueue.Enqueue(deserializedCommand);
                }

            }
            TimeSpan timespan = IoC.Resolve<TimeSpan>("DeserializeTimespan", serializedData[3]);

            var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            var initialScope = IoC.Resolve<object>("ThreadScope.Current", threadId);
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();
            
            IoC.Resolve<Interfaces.ICommand>("Scope.Current.RegisterOptions", gameOptions).Execute();
            IoC.Resolve<Interfaces.ICommand>("Game.AddObjects", gameObjects).Execute();
            IoC.Resolve<Interfaces.ICommand>("QuantumForGame.Set").Execute();

            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", initialScope).Execute();
            string idForNewGame = IoC.Resolve<string>("ThreadScope.GameId.New");
            Interfaces.ICommand newGameCommand = new MacroGameCommand(idForNewGame, gameScope, gameQueue);
            ISender senderToCurrentThreadQueue = IoC.Resolve<ISender>("MySender");
            senderToCurrentThreadQueue.Send(newGameCommand);
        }
    }
}
