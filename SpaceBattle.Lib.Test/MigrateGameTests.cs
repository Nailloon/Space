
using System.Collections.Concurrent;
using SpaceBattle.gRPC.Others;
using SpaceBattle.gRPC.Router;
using SpaceBattle.gRPC.Services;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using SpaceBattle.ServerStrategies;
using SpaceBattle.SuperGameCommand;

namespace SpaceBattle.Lib.Test;

public class MigrateGameTest
{
    public MigrateGameTest()
    {
    




        BlockingCollection<ICommand> queue1 = new();
        BlockingCollection<ICommand> orderQueue1 = new();
        ISender orderSender1 = new SenderAdapter(orderQueue1);
        IReceiver orderReceiver1 = new ReceiverAdapter(orderQueue1);
        ISender sender1 = new SenderAdapter(queue1);
        IReceiver receiver1 = new ReceiverAdapter(queue1);
        var th2 = Hwdtech.IoC.Resolve<MyThread>("CreateAndStartThread", "80", sender1, receiver1, orderSender1, orderReceiver1);

        IRouter router1 = new Router(gamesThreadsDictionary, orderSenderDict);
        EndPointService endpoint1 = new EndPointService(router1);

        "DeserializeValue"
        "DeserializeTimespan"
        "ThreadScope.Current"
        "Scopes.Current.Set"
        "Scope.Current.RegisterOptions"
        "Game.AddObjects"
        "QuantumForGame.Set"
        "ThreadScope.GameId.New"
        "MySender"
    }
    [Fact]
    public void PositiveSerializeGame()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();

        var threadDict = new ConcurrentDictionary<string, MyThread>();
        var gamesThreadsDictionary = new ConcurrentDictionary<string, string>();
        var senderDict = new ConcurrentDictionary<string, ISender>();
        var orderSenderDict = new ConcurrentDictionary<string, ISender>();
        var gamesDictionary = new Dictionary<string, Queue<ICommand>>();
        Queue<ICommand> gameQueue1 = new Queue<ICommand>();
        gamesDictionary.TryAdd("1", gameQueue1);
        ICommand gameCommand1 = new GameCommand("1", gameQueue1);
        gamesThreadsDictionary.TryAdd("1", "80");

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDMyThreadMapping", (object[] _) => threadDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDOrdersSenderMapping", (object[] _) => orderSenderDict).Execute();

        var createAndStartThreadStrategy = new CreateAndStartThreadStrategy();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateAndStartThread", (object[] args) => createAndStartThreadStrategy.StartStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 40, 0)).Execute();
        var protobufMapToDictionaryStrategy = new ProtobufMapToDictionaryStrategy();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ProtobufMapToDictionary", (object[] args) => protobufMapToDictionaryStrategy.StartStrategy(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandToGame", (object[] args) => gamesDictionary[(string)args[0]].Append((ICommand)args[1])).Execute();

        BlockingCollection<ICommand> queue = new();
        BlockingCollection<ICommand> orderQueue = new();
        ISender orderSender = new SenderAdapter(orderQueue);
        IReceiver orderReceiver = new ReceiverAdapter(orderQueue);
        ISender sender = new SenderAdapter(queue);
        IReceiver receiver = new ReceiverAdapter(queue);
        var th1 = Hwdtech.IoC.Resolve<MyThread>("CreateAndStartThread", "80", sender, receiver, orderSender, orderReceiver);

        IRouter router = new Router(gamesThreadsDictionary, orderSenderDict);
        EndPointService endpoint = new EndPointService(router);

        endpoint.MigrateGame()

        "StopGameCommandExecuting"
        "Game.Options.GetAll"
        "Game.Objects.GetAll"
        "Game.Queue.Get"
        "Game.Get.Timespan"
        "StringfyOption"
        "StringifyObject"
        "SerializeCommand"

    }
}
