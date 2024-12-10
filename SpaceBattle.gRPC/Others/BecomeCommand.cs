namespace SpaceBattle.gRPC.Others;

using Hwdtech;
using ICommand = SpaceBattle.Interfaces.ICommand;
using IStrategy = SpaceBattle.Interfaces.IStrategy;


    class BecomeCommand : ICommand
    {
        string _gameId;
        Google.Protobuf.Collections.MapField<string, string> _orderMap;
        public BecomeCommand(string gameId, Google.Protobuf.Collections.MapField<string, string> orderMap){
            _gameId = gameId;
            _orderMap = orderMap;
        }
        public void Execute()
        {
            Dictionary<string,string> orderProperties = IoC.Resolve<Dictionary<string,string>>("ProtobufMapToDictionary", _orderMap);
            ICommand command = IoC.Resolve<ICommand>("OrderDictionaryToICommand", orderProperties);
            IoC.Resolve<IStrategy>("SendCommandToGame", _gameId, command).StartStrategy();
        }
    }