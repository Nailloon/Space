using System.Collections.Concurrent;
using SpaceBattle.gRPC;
using SpaceBattle.gRPC.Others;
using SpaceBattle.gRPC.Services;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using SpaceBattle.ServerStrategies;
using SpaceBattle.SuperGameCommand;
using IRouter = SpaceBattle.gRPC.IRouter;

new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();

var threadDict = new ConcurrentDictionary<string, MyThread>();
var gamesThreadsDictionary = new ConcurrentDictionary<string, string>();
var senderDict = new ConcurrentDictionary<string, ISender>();
var orderSenderDict = new ConcurrentDictionary<string, ISender>();
var gamesDictionary = new Dictionary<string, Queue<ICommand>>();
Queue<ICommand> gameQueue1 = new Queue<ICommand>();
Queue<ICommand> gameQueue2 = new Queue<ICommand>();
gamesDictionary.TryAdd("1", gameQueue1);
gamesDictionary.TryAdd("2", gameQueue2);
ICommand gameCommand1 = new GameCommand("1", gameQueue1);
ICommand gameCommand2 = new GameCommand("2", gameQueue2);
gamesThreadsDictionary.TryAdd("1", "80");

Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDMyThreadMapping", (object[] _) => threadDict).Execute();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDOrdersSenderMapping", (object[] _) => orderSenderDict).Execute();
var createAllStrategy = new CreateAllStrategy();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateAll", (object[] args) => createAllStrategy.StartStrategy(args)).Execute();
var createAndStartThreadStrategy = new CreateAndStartThreadStrategy();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateAndStartThread", (object[] args) => createAndStartThreadStrategy.StartStrategy(args)).Execute();
var createReceiverAdapterStrategy = new CreateReceiverAdapterStrategy();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateReceiverAdapter", (object[] args) => createReceiverAdapterStrategy.StartStrategy(args)).Execute();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] _) => (object)new TimeSpan(0, 0, 0, 40, 0)).Execute();
var protobufMapToDictionaryStrategy = new ProtobufMapToDictionaryStrategy();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ProtobufMapToDictionary", (object[] args) => protobufMapToDictionaryStrategy.StartStrategy(args)).Execute();
ICommand emptyCommand = new ActionCommand(()=>{});
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "OrderDictionaryToICommand", (object[] args) => emptyCommand).Execute();
Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandToGame", (object[] args) => gamesDictionary[(string)args[0]].Append((ICommand)args[1])).Execute();

var th1 = Hwdtech.IoC.Resolve<MyThread>("CreateAll", "80");
IRouter router = new Router(gamesThreadsDictionary, orderSenderDict);
EndPointService endpoint = new EndPointService(router);

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton(endpoint);
builder.Services.AddGrpc();

WebApplication app = builder.Build();
app.MapGrpcService<EndPointService>();
app.Run();
