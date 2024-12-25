using Hwdtech;
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
using SpaceBattle.gRPC;

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
        var senderOrderDict = new ConcurrentDictionary<string, ISender>();
        var gamesDictionary = new Dictionary<string, Queue<ICommand>>();
        Queue<ICommand> gameQueue1 = new Queue<ICommand>();
        gamesDictionary.TryAdd("1", gameQueue1);
        Queue<ICommand> gameQueue2 = new Queue<ICommand>();
        gamesDictionary.TryAdd("2", gameQueue2);
        ICommand gameCommand1 = new GameCommand("1", gameQueue1);
        ICommand gameCommand2 = new GameCommand("2", gameQueue2);
        gamesThreadsDictionary.TryAdd("1", "80");
        gamesThreadsDictionary.TryAdd("2", "80");

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDMyThreadMapping", (object[] _) => threadDict).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDOrdersSenderMapping", (object[] _) => senderOrderDict).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

        var createWithStartThreadStrategy = new CreateAndStartThreadStrategy();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateWithStartThread", (object[] args) => createWithStartThreadStrategy.StartStrategy(args)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 0, 150)).Execute();
        var command1 = new Mock<ICommand>();
            var regStrategy1 = new Mock<IStrategy>();
            command1.Setup(_command => _command.Execute());
            regStrategy1.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(command1.Object);

        BlockingCollection<ICommand> queue = new();
        BlockingCollection<ICommand> orderQueue = new();
        ISender orderSender = new SenderAdapter(orderQueue);
        IReceiver orderReceiver = new ReceiverAdapter(orderQueue);
        ISender sender = new SenderAdapter(queue);
        IReceiver receiver = new ReceiverAdapter(queue);

        var mre0 = new ManualResetEvent(false);
        var mre1 = new ManualResetEvent(false);

        var valueMap = new Google.Protobuf.Collections.MapField<string, string>(){{"type", "Move"},{"objid", "uobj1"},{"velocity", "5"}};
        OrderRequest orderRequest = new()
        {
            GameId = "2"
        };
        orderRequest.Map.Add(valueMap);

        var MockCommand = new Mock<ICommand>();
        MockCommand.Setup(x => x.Execute());
                            ICommand commandForGame = new ActionCommand(()=>{
                        MockCommand.Object.Execute();
        });
        IRouter router = new Router(gamesThreadsDictionary, senderOrderDict);
        Action act1 = () => {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "OrderDictionaryToICommand", (object[] args) => commandForGame).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandToGame", (object[] args) => new ActionCommand(()=>gameQueue2.Append((ICommand)args[1]))).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.StartStrategy(args)).Execute();
                mre0.Set();
            };
        var ST = new MyThread(receiver, orderReceiver);
        senderDict.TryAdd("80", sender);
        threadDict.TryAdd("80", ST);
        senderOrderDict.TryAdd("80", orderSender);
        sender.Send(new ActionCommand(act1));
        ST.Execute();

        mre0.WaitOne();
        router.route(orderRequest.GameId, orderRequest.Map);
        sender.Send(gameCommand1);
        sender.Send(gameCommand2);
        sender.Send(new ActionCommand(()=>{mre1.Set();}));
        mre1.WaitOne();
        Assert.Empty(orderQueue);
        Assert.Empty(gameQueue2);
        MockCommand.Verify();
    }
}