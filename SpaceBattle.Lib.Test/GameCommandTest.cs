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
using System.ComponentModel;

namespace SpaceBattle.Lib.Test
{
    [Collection("Sequential")]
    public class GameCommandTest
    {
        public GameCommandTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var initialScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", initialScope).Execute();

            var threadDict = new ConcurrentDictionary<string, MyThread>();
            var senderDict = new ConcurrentDictionary<string, ISender>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ReturnSenderDict", (object[] _) => { return senderDict; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ReturnInitialScope", (object[] _) => { return initialScope; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDMyThreadMapping", (object[] _) => threadDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderAdapterGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

            var createAllStrategy = new CreateAllStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateAll", (object[] args) => createAllStrategy.StartStrategy(args)).Execute();
            var createAndStartThreadStrategy = new CreateAndStartThreadStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateAndStartThread", (object[] args) => createAndStartThreadStrategy.StartStrategy(args)).Execute();
            var createReceiverAdapterStrategy = new CreateReceiverAdapterStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateReceiverAdapter", (object[] args) => createReceiverAdapterStrategy.StartStrategy(args)).Execute();

            var sendCommandStrategy = new SendCommandStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCommandStrategy.StartStrategy(args)).Execute();

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
        public void GameCommandWithoutExceptionSuccessfulTest()
        {
            var senderDict = IoC.Resolve<ConcurrentDictionary<string, ISender>>("ReturnSenderDict");
            var threadGameDict = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
            var scopeGameDict = new ConcurrentDictionary<string, object>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ScopeByGameID", (object[] args) => scopeGameDict).Execute();
            var sendCommandByThreadID = new SendCommandByThreadIDStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandByThreadID", (object[] args) => sendCommandByThreadID.StartStrategy(args)).Execute();
            var scopesByGameid = IoC.Resolve<ConcurrentDictionary<string, object>>("Storage.ScopeByGameID");
            var threadScope = IoC.Resolve<object>("ReturnInitialScope");
            var mre1 = new ManualResetEvent(false);
            var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            scopesByGameid.TryAdd("game1", gameScope);
            var handleExceptionStrategy = new HandleExceptionStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => handleExceptionStrategy.StartStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadScope.Current", (object[] args) =>
            {
                var scopes = IoC.Resolve<ConcurrentDictionary<string, object>>("Storage.ScopeByGameID");
                return scopes[(string)args[0]];
            }).Execute();

            var th1 = IoC.Resolve<MyThread>("CreateAll", "thread1");
            IoC.Resolve<ICommand>("SendCommandByThreadID", "thread1", new ActionCommand(() => {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();
                var quantum = new TimeSpan(0, 0, 0, 0, 100);
                var quantumStrategy = new Mock<IStrategy>();
                quantumStrategy.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(quantum);
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "QuantumForGame", (object[] args) => quantumStrategy.Object.StartStrategy()).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.ScopeByGameID", (object[] args) => scopeGameDict).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommandByThreadIDStrategy", (object[] args) => sendCommandByThreadID.StartStrategy(args)).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HandleException", (object[] args) => handleExceptionStrategy.StartStrategy(args)).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Storage.GetThreadByGameID", (object[] args) =>
                {
                    return threadGameDict[(string)args[0]];
                }
                ).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ThreadScope.Current", (object[] args) =>
                {
                    return scopesByGameid[(string)args[0]];
                }
                ).Execute();
                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SenderAdapterGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", threadScope).Execute();
                mre1.Set();
            })).Execute();
            mre1.WaitOne();

            var repeatGameCommand = new RepeatGameCommand("game1", gameScope);
            var games = IoC.Resolve<ConcurrentDictionary<string, string>>("Storage.ThreadByGameID");
            games.TryAdd("game1", "thread1");
            var mre2 = new ManualResetEvent(false);
            IoC.Resolve<ICommand>("SendCommandByThreadID", "thread1", repeatGameCommand).Execute();
            IoC.Resolve<ICommand>("SendCommandByThreadID", "thread1", new ActionCommand(() => { mre2.Set(); })).Execute();
            Assert.False(th1.QueueIsEmpty());
            mre2.WaitOne();
        }
    }
}
