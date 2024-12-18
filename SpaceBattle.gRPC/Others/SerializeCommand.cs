
using Hwdtech;
using SpaceBattle.gRPC.Client;

namespace SpaceBattle.gRPC.Others
{
    public class SerializeCommand : Interfaces.ICommand
    {
        private string gameId;
        private string serializedString ="";
        private string newServerId;

        public SerializeCommand(string gameId, string newServerId)
        {
            this.gameId = gameId;
            this.newServerId = newServerId;
        }

        public void Execute()
        {
            IoC.Resolve<Interfaces.ICommand>("StopGameCommandExecuting").Execute();
            List<string> gameOptions= IoC.Resolve<List<string>>("Game.Options.GetAll", gameId);
            Dictionary<string, object> gameObjects = IoC.Resolve<Dictionary<string, object>>("Game.Objects.GetAll", gameId);
            Queue<Interfaces.ICommand> gameQueue = IoC.Resolve<Queue<Interfaces.ICommand>>("Game.Queue.Get", gameId);
            TimeSpan timespan = IoC.Resolve<TimeSpan>("Game.Get.Timespan", gameId);

            foreach(string option in gameOptions)
            {
                serializedString += IoC.Resolve<string>("StringfyOption", option);
            }
            
            serializedString += " | ";

            foreach(KeyValuePair<string, object> entry in gameObjects)
            {
                serializedString += entry.Key + " : " + IoC.Resolve<string>("StringifyObject", entry.Value) + ";";
            }

            serializedString += " | ";

            foreach(Interfaces.ICommand cmd in gameQueue.ToArray()){
                serializedString += IoC.Resolve<string>("SerializeCommand", cmd);
            }

            serializedString += " | ";

            serializedString += timespan.ToString();

            EndPointClient.Call(newServerId, serializedString);
        }
    }
}
