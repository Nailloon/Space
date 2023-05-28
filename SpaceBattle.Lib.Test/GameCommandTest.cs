using Hwdtech.Ioc;
using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System.Collections.Concurrent;
using SpaceBattle.ServerStrategies;
using SpaceBattle.gRPC.Services;
using Moq;
using SpaceBattle.Exceptions;
using SpaceBattle.SuperGameCommand;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.Lib.Test
{
    [Collection("Sequential")]
    public class GameCommandTest
    {
        public GameCommandTest()
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

            var threadgamedict = new ConcurrentDictionary<string, string>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ThreadByGameID", (object[] args) => threadgamedict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.GetThreadByGameID", (object[] args) =>
            {
                var dict = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
                return dict[(string)args[0]];
            }
            ).Execute();
            var command1 = new Mock<Interfaces.ICommand>();
            var regStrategy1 = new Mock<IStrategy>();
            command1.Setup(_command => _command.Execute()).Verifiable();
            regStrategy1.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(command1.Object).Verifiable();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.StartStrategy(args)).Execute();
            var quantum = new TimeSpan(0, 0, 0, 0, 100);
            var quantumStrategy = new Mock<IStrategy>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] args) => quantumStrategy.Object.StartStrategy(args)).Execute();
        }
        [Fact]
        public void GameCommandWithoutExceptionSuccessfulTest()
        {
            var scopeGameDict = new ConcurrentDictionary<string, object>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ScopeByGameID", (object[] args) => scopeGameDict).Execute();
            var sendCommandByThreadID = new SendCommandByThreadIDStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandByThreadID", (object[] args) => sendCommandByThreadID.StartStrategy(args)).Execute();
            var scopesByGameid = IoC.Resolve<ConcurrentDictionary<string, object>>("Storage.ScopeByGameID");
            var threadScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            scopesByGameid.TryAdd("game1", threadScope);
            var mre1 = new ManualResetEvent(false);

            Action act2 = () =>
            {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", threadScope).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadScope.Current", (object[] args) =>
                {
                    var scopes = IoC.Resolve<ConcurrentDictionary<string, object>>("Storage.ScopeByGameID");
                    return scopes[(string)args[0]];
                }
                ).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.GetThreadByGameID", (object[] args) =>
                {
                    var dict = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
                    return dict[(string)args[0]];
                }
                ).Execute();
                var quantum = new TimeSpan(0, 0, 0, 0, 100);
                var quantumStrategy = new Mock<IStrategy>();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] args) => quantumStrategy.Object.StartStrategy(args)).Execute();
                quantumStrategy.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(quantum).Verifiable();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ScopeByGameID", (object[] args) => scopeGameDict).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] args) => quantumStrategy.Object.StartStrategy(args)).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandByThreadID", (object[] args) => sendCommandByThreadID.StartStrategy(args)).Execute();
                var handleExceptionStrategy = new HandleExceptionStrategy();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameHandleException", (object[] args) => handleExceptionStrategy.StartStrategy(args)).Execute();
                var command2 = new Mock<Interfaces.ICommand>();
                var regStrategy2 = new Mock<IStrategy>();
                command2.Setup(_command => _command.Execute()).Verifiable();
                regStrategy2.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(command2.Object).Verifiable();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy2.Object.StartStrategy(args)).Execute();
                mre1.Set();
            };
            var th1 = IoC.Resolve<MyThread>("CreateAll", "thread1", act1);
            mre1.WaitOne();

            var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));

            var repeatGameCommand = new RepeatGameCommand("game1", gameScope);
            var games = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
            games.TryAdd("game1", "thread1");
            var mre2 = new ManualResetEvent(false);
            IoC.Resolve<ICommand>("SendCommandByThreadID", "thread1", repeatGameCommand).Execute();
            IoC.Resolve<ICommand>("SendCommandByThreadID", "thread1", new ActionCommand(() => { mre2.Set(); })).Execute();
            mre2.WaitOne();
            Assert.False(th1.QueueIsEmpty());
        }
    }
}
