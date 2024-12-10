using Hwdtech.Ioc;
using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System.Collections.Concurrent;
using SpaceBattle.gRPC;
using Moq;
using ICommand = SpaceBattle.Interfaces.ICommand;
using SpaceBattle.ServerStrategies;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using SpaceBattle.gRPC.Services;
using SpaceBattleGrpc.Others;
namespace SpaceBattle.Lib.Test
{
    public class gRPCTest
    {
        public gRPCTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            var threadDict = new ConcurrentDictionary<string, MyThread>();
            var senderDict = new ConcurrentDictionary<string, ISender>();
            var orderDict = new ConcurrentDictionary<string, ISender>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDMyThreadMapping", (object[] _) => threadDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDOrdersSenderMapping", (object[] _) => orderDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderAdapterGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

            var sendCommandStrategy = new SendCommandStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCommandStrategy.StartStrategy(args)).Execute();

            var createAllStrategy = new CreateAllStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateAll", (object[] args) => createAllStrategy.StartStrategy(args)).Execute();
            var createAndStartThreadStrategy = new CreateAndStartThreadStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateAndStartThread", (object[] args) => createAndStartThreadStrategy.StartStrategy(args)).Execute();
            var createReceiverAdapterStrategy = new CreateReceiverAdapterStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateReceiverAdapter", (object[] args) => createReceiverAdapterStrategy.StartStrategy(args)).Execute();
            var hardStopStrategy = new HardStopStrategy();

            var threadgamedict = new ConcurrentDictionary<string, string>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ThreadByGameID", (object[] args) => threadgamedict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.GetThreadByGameID", (object[] args) =>
            {
                var dict = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
                return dict[(string)args[0]];
            }
            ).Execute();
            var sendCommandByThreadID = new SendCommandByThreadIDStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandByThreadID", (object[] args) => sendCommandByThreadID.StartStrategy(args)).Execute();
        }

        [Fact]
        public void EndPointSuccessfulTest()
        {
            var cestrat = new StartEndPointServiceStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateEndPoint", (object[] args) => cestrat.StartStrategy(args)).Execute();

            var thread1 = IoC.Resolve<MyThread>("CreateAll", "thread1");

            var games = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
            games.TryAdd("game1", "thread1");
            var request = new CommandRequest { GameId = "game1", CommandType = "Check", GameItemId = "1" };
            var d = new Dictionary<string, string>() { { "123", "456" }, { "12", "24" } };
            var commandArgs = d.Select(kv => new CommandForObject { Key = kv.Key, Value = kv.Value }).ToArray();
            request.Args.Add(commandArgs);
            var cmd = new Mock<ICommand>();
            cmd.Setup(_command => _command.Execute()).Verifiable();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateCommandByNameForObject", (object[] args) =>
            {
                return cmd.Object;
            }
            ).Execute();
            var mre1 = new ManualResetEvent(false);
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "thread1");
            var endp = IoC.Resolve<ICommand>("CreateEndPoint");
            var service = new EndPointService(new Mock<ILogger<EndPointService>>().Object, new Mock<IRouter>().Object);
            service.Command(request, new Mock<ServerCallContext>().Object);
            IoC.Resolve<ICommand>("SendCommand", sender, new ActionCommand(() => { mre1.Set(); })).Execute();
            mre1.WaitOne();
            Assert.True(thread1.QueueIsEmpty());
            cmd.Verify();
        }
    }
}
