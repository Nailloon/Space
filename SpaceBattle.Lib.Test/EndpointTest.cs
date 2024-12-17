using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using System.Collections.Concurrent;
using SpaceBattle.Interfaces;
using ICommand = SpaceBattle.Interfaces.ICommand;
using SpaceBattle.Server;
using SpaceBattle.SuperGameCommand;
using SpaceBattle.ServerStrategies;
using SpaceBattle.gRPC.Services;
using SpaceBattle.gRPC.Router;
using SpaceBattle.gRPC.Others;

namespace SpaceBattle.Lib.Test;

public class EndpointTest
{
    [Fact]
    public void PositiveRoutingTest()
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

    var valueMap = new Google.Protobuf.Collections.MapField<string, string>(){{"type", "Move"},{"objid", "uobj1"},{"velocity", "5"}};
    ICommand commandForGame = new ActionCommand(()=>{
        var uobj = IoC.Resolve<IUObject>("GetUObject", "uobj1");
        uobj.set_property("MoveVelocity", 5);
    });

    Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "OrderDictionaryToICommand", (object[] args) => commandForGame).Execute();

    IRouter router = new Router(gamesThreadsDictionary, orderSenderDict);
    EndPointService endpoint = new EndPointService(router);

        Assert.Empty(gameQueue1);
        router.route("1", valueMap);
        Assert.NotEmpty(orderQueue);
    }


}