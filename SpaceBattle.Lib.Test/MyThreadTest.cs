using Hwdtech.Ioc;
using Hwdtech;
using System.Collections.Concurrent;
using SpaceBattle.Server;
using SpaceBattle.ServerStrategies;
using SpaceBattle.Interfaces;
using ICommand = Hwdtech.ICommand;

namespace SpaceBattle.Lib.Test
{
    public class MyThreadTest
    {
        public MyThreadTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            var threadDict = new ConcurrentDictionary<string, MyThread>();
            var senderDict = new ConcurrentDictionary<string, ISender>();
            IoC.Resolve<ICommand>("IoC.Register", "ThreadIDMyThreadMapping", (object[] _) => threadDict).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "ThreadIDSenderMapping", (object[] _) => senderDict).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "SenderAdapterGetByID", (object id) => senderDict[(string)id]).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "ServerThreadGetByID", (object id) => threadDict[(string)id]).Execute();

            var createAllStrategy = new CreateAllStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "CreateAll", (object[] args) => createAllStrategy.StartStrategy(args)).Execute();
            var createAndStartThreadStrategy = new CreateAndStartThreadStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "CreateAndStartThread", (object[] args) => createAndStartThreadStrategy.StartStrategy(args)).Execute();
            var createReceiverAdapterStrategy = new CreateReceiverAdapterStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "CreateReceiverAdapter", (object[] args) => createReceiverAdapterStrategy.StartStrategy(args)).Execute();
            var hardStopStrategy = new HardStopStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "HardStopCommand", (object[] args) => hardStopStrategy.StartStrategy(args)).Execute();
            var softStopStrategy = new SoftStopStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "SoftStopCommand", (object[] args) => softStopStrategy.StartStrategy(args)).Execute();
            var sendCommandStrategy = new SendCommandStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCommandStrategy.StartStrategy(args)).Execute();

            //HandleExceptionStrategy
        }
        //[Fact]
        //public void MyThreadHardStopTest()
        //{

        //}
        //[Fact]
        public void MyThreadSoftStopTest()
        {
            BlockingCollection<SpaceBattle.Interfaces.ICommand> que1 = new BlockingCollection<SpaceBattle.Interfaces.ICommand>(100);
            var sender1 = new SenderAdapter(que1);
            BlockingCollection<SpaceBattle.Interfaces.ICommand> que2 = new BlockingCollection<SpaceBattle.Interfaces.ICommand>(100);
            var sender2 = new SenderAdapter(que2);
        }
        [Fact]
        public void MyThreadCreateTest()
        {
            var Th1 = IoC.Resolve<MyThread>("CreateAll", "83675");
            var Th2 = IoC.Resolve<MyThread>("CreateAll", "83675", (() => Thread.Sleep(5000)));
            Assert.False(Th1 == Th2);
            Assert.False(Th1.Equals(Th2));
        }
        //[Fact]
        //public void MyThreadUpdateBehaviorTest()
        //{

        //}
        //[Fact]
        //public void MyThreadWorkingTogetherTest()
        //{

        //}
    }
}
