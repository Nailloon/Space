using Hwdtech.Ioc;
using Hwdtech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using SpaceBattleGrpc.Strategies;

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
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDMyThreadMapping", (object[] _) => threadDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
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
        }

        [Fact]
        public void CommandSuccessfulTestWithoutException()
        {
            var mockCommand1 = new Mock<ICommand>();
            mockCommand1.Setup(_command => _command.Execute()).Verifiable();
            var moveStrategyForObject = new Mock<IStrategy>();
            moveStrategyForObject.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(mockCommand1.Object).Verifiable();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateCommandByNameForObject", (object[] args) => moveStrategyForObject.Object.StartStrategy(args)).Execute();

            var command1 = new CommandForObject
            {
                GameItemId = "123",
                Value = "1"
            };
            var command2 = new CommandForObject
            {
                GameItemId = "124",
                Value = "2"
            };
            var args = new List<CommandForObject>();
            args.Add(command1);
            args.Add(command2);
            var request = new CommandRequest
            {
                GameId = "456",
                CommandType = "Move",
                Args = { args }
            };
            var mre1 = new ManualResetEvent(false);
            var th1 = IoC.Resolve<MyThread>("CreateAll", "456", () => { mre1.Set(); });
            var cps = new EndPointService(new Mock<ILogger<EndPointService>>().Object);
            cps.Command(request, new Mock<ServerCallContext>().Object);
            var mre2 = new ManualResetEvent(false);
            IoC.Resolve<ICommand>("SendCommand", IoC.Resolve<ISender>("SenderAdapterGetByID", "456"), new ActionCommand(() => { mre2.Set(); })).Execute();
            mre1.WaitOne(200);
            moveStrategyForObject.Verify();
            mre2.WaitOne(200);
            mockCommand1.Verify();
            Assert.True(th1.QueueIsEmpty());
        }
        [Fact]
        public void EndPointSuccessfulTest()
        {
            var cestrat = new StartEndPointServiceStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateEndPoint", (object[] args) => cestrat.StartStrategy(args)).Execute();

            var thread1 = IoC.Resolve<MyThread>("CreateAll", "thread1");

            var games = IoC.Resolve < ConcurrentDictionary < string, string>>("Storage.ThreadByGameID");
            games.TryAdd("game1", "thread1");
            var request = new CommandRequest { GameId= "game1", CommandType= "Check" };
            var d = new Dictionary<string, string>() { { "123", "456" }, { "12", "24" } };
            var commandArgs = d.Select(kv => new CommandForObject { GameItemId = kv.Key, Value = kv.Value }).ToArray();
            request.Args.Add(commandArgs);
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateCommandByNameForObject", (object[] args) =>
            {
                var cmd = new Mock<ICommand>();
                Assert.Same(request.CommandType, args[0]);
                Assert.Equal(d.Keys.ToArray(), args[1]);
                Assert.Equal(d.Values.ToArray(), args[2]);
                return cmd.Object;
            }
            ).Execute();
            //Act
            var endp = IoC.Resolve<ICommand>("CreateEndPoint");
            var service = new EndPointService(new Mock<ILogger<EndPointService>>().Object);
            service.Command(request, new Mock<ServerCallContext>().Object);
            Assert.False(thread1.QueueIsEmpty());
        }
    }
}
