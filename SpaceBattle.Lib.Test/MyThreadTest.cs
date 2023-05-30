using Hwdtech.Ioc;
using Hwdtech;
using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using SpaceBattle.ServerStrategies;
using System.Collections.Concurrent;
using ICommand = Hwdtech.ICommand;
using System.Reflection;

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
            IoC.Resolve<ICommand>("IoC.Register", "SenderAdapterGetByID", (object[] id) => senderDict[(string)id[0]]).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "ServerThreadGetByID", (object[] id) => threadDict[(string)id[0]]).Execute();

            var createAllStrategy = new CreateAllStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "CreateAll", (object[] args) => createAllStrategy.StartStrategy(args)).Execute();
            var createAndStartThreadStrategy = new CreateAndStartThreadStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "CreateAndStartThread", (object[] args) => createAndStartThreadStrategy.StartStrategy(args)).Execute();
            var createReceiverAdapterStrategy = new CreateReceiverAdapterStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "CreateReceiverAdapter", (object[] args) => createReceiverAdapterStrategy.StartStrategy(args)).Execute();
            var hardStopStrategy = new HardStopStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "HardStop", (object[] args) => hardStopStrategy.StartStrategy(args)).Execute();
            var softStopStrategy = new SoftStopStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "SoftStop", (object[] args) => softStopStrategy.StartStrategy(args)).Execute();
            var sendCommandStrategy = new SendCommandStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCommandStrategy.StartStrategy(args)).Execute();
            var macroCommandForHardStopStrategy = new MacroCommandForHardStopStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "MacroCommandForHardStopStrategy", (object[] args) => macroCommandForHardStopStrategy.StartStrategy(args)).Execute();
            var commandForSoftStopStrategy = new CommandForSoftStopStrategy();
            IoC.Resolve<ICommand>("IoC.Register", "CommandForSoftStopStrategy", (object[] args) => commandForSoftStopStrategy.StartStrategy(args)).Execute();
        }
        [Fact(Timeout = 1500)]
        public void MyThreadHardStopTest()
        {
            var th3 = IoC.Resolve<MyThread>("CreateAll", "83673");
            var mre1 = new ManualResetEvent(false);
            Assert.True(th3.QueueIsEmpty());
            var hardStopCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HardStop", "83673", () => { mre1.Set(); });
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "83673");
            var sendCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, hardStopCommand);
            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th3.QueueIsEmpty());
            Assert.True(th3.GetStop());
        }
        [Fact(Timeout = 1500)]
        public void MyThreadSoftStopTest()
        {
            var mre1 = new ManualResetEvent(false);
            var th1 = IoC.Resolve<MyThread>("CreateAll", "83674");
            Assert.True(th1.QueueIsEmpty());
            var softStopCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SoftStop", "83674", () => { mre1.Set(); });
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "83674");
            var sendCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, softStopCommand);
            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th1.QueueIsEmpty());
            Assert.True(th1.GetStop());
        }
        [Fact(Timeout = 1500)]
        public void MyThreadCreateTest()
        {
            BlockingCollection<SpaceBattle.Interfaces.ICommand> que = new BlockingCollection<SpaceBattle.Interfaces.ICommand>(100);
            var sender = new SenderAdapter(que);
            var receiver = IoC.Resolve<IReceiver>("CreateReceiverAdapter", que);
            var MT = IoC.Resolve<MyThread>("CreateAndStartThread", "78", sender, receiver);
            Assert.True(MT.QueueIsEmpty());
            Assert.False(MT.GetStop());
            Assert.NotNull(IoC.Resolve<MyThread>("ServerThreadGetByID", "78"));
            Assert.NotNull(IoC.Resolve<ISender>("SenderAdapterGetByID", "78"));
        }
        [Fact(Timeout = 1500)]
        public void MyThreadEqualsTrueTest()
        {
            var mre1 = new ManualResetEvent(false);
            BlockingCollection<SpaceBattle.Interfaces.ICommand> que = new BlockingCollection<SpaceBattle.Interfaces.ICommand>(100);
            var sender1 = new SenderAdapter(que);
            var receiver1 = IoC.Resolve<IReceiver>("CreateReceiverAdapter", que, () => { mre1.Set(); });
            var Th1 = IoC.Resolve<MyThread>("CreateAndStartThread", "8367", sender1, receiver1);
            Assert.True(Th1.Equals(IoC.Resolve<MyThread>("ServerThreadGetByID", "8367")));
            mre1.WaitOne(200);

        }
        [Fact(Timeout = 1500)]
        public void MyThreadEqualsFalseTest()
        {
            var Th1 = IoC.Resolve<MyThread>("CreateAll", "8367");
            var Th2 = IoC.Resolve<MyThread>("CreateAll", "8376");
            Assert.True(Th1.Equals(IoC.Resolve<MyThread>("ServerThreadGetByID", "8367")));
            Assert.False(Th2.Equals(IoC.Resolve<MyThread>("ServerThreadGetByID", "8367")));
        }
        [Fact(Timeout = 1500)]
        public void MyThreadHardStopTestWithException()
        {
            var command1 = new Mock<Interfaces.ICommand>();
            var regStrategy1 = new Mock<IStrategy>();
            command1.Setup(_command => _command.Execute()).Verifiable();
            regStrategy1.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(command1.Object).Verifiable();
            IoC.Resolve<ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.StartStrategy(args)).Execute();
            Action act1 = () => {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                IoC.Resolve<ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.StartStrategy(args)).Execute();
            };

            var th3 = IoC.Resolve<MyThread>("CreateAll", "83673", act1);
            var th6 = IoC.Resolve<MyThread>("CreateAll", "835", act1);
            var mre1 = new ManualResetEvent(false);
            var hardStopCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HardStop", "835", () => { mre1.Set(); });
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "83673");
            var sendCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, hardStopCommand);

            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th3.QueueIsEmpty());
            Assert.False(th3.GetStop());
            Assert.False(th6.GetStop());
        }
        [Fact(Timeout = 1500)]
        public void MyThreadSoftStopWorkingSomeTime()
        {
            var mockCommand1 = new Mock<Interfaces.ICommand>();
            var mockCommand3 = new Mock<Interfaces.ICommand>();
            var mockCommand4 = new Mock<Interfaces.ICommand>();
            var mockCommand2 = new Mock<Interfaces.ICommand>();
            mockCommand1.Setup(_command => _command.Execute()).Verifiable();
            mockCommand3.Setup(_command => _command.Execute()).Verifiable();
            mockCommand4.Setup(_command => _command.Execute()).Verifiable();
            mockCommand2.Setup(_command => _command.Execute()).Verifiable();

            var mre1 = new ManualResetEvent(false);
            var th1 = IoC.Resolve<MyThread>("CreateAll", "83674");
            Assert.True(th1.QueueIsEmpty());
            var softStopCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SoftStop", "83674");
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "83674");
            IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, mockCommand1.Object).Execute();
            IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, mockCommand2.Object).Execute();
            var sendCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, softStopCommand);
            sendCommand.Execute();
            IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, mockCommand3.Object).Execute();
            IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, mockCommand4.Object).Execute();
            IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, new ActionCommand(() => { mre1.Set(); })).Execute();
            Assert.False(th1.QueueIsEmpty());
            mre1.WaitOne(200);
            mockCommand1.Verify();
            mockCommand3.Verify();
            mockCommand4.Verify();
            mockCommand2.Verify();
            Assert.True(th1.QueueIsEmpty());
            Assert.True(th1.GetStop());
        }
        [Fact(Timeout=1500)]
        public void MyThreadSoftStopTestWithException()
        {
            var command1 = new Mock<Interfaces.ICommand>();
            var regStrategy1 = new Mock<IStrategy>();
            command1.Setup(_command => _command.Execute()).Verifiable();
            regStrategy1.Setup(_strategy => _strategy.StartStrategy(It.IsAny<object[]>())).Returns(command1.Object).Verifiable();
            IoC.Resolve<ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.StartStrategy(args)).Execute();
            Action act1 = () => {
                IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
                IoC.Resolve<ICommand>("IoC.Register", "HandleException", (object[] args) => regStrategy1.Object.StartStrategy(args)).Execute();
            };

            var th10 = IoC.Resolve<MyThread>("CreateAll", "83673", act1);
            var th8 = IoC.Resolve<MyThread>("CreateAll", "835", act1);
            var mre1 = new ManualResetEvent(false);
            var softStopCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SoftStop", "835", () => { mre1.Set(); });
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "83673");
            var sendCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, softStopCommand);

            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th10.QueueIsEmpty());
            Assert.False(th10.GetStop());
            Assert.False(th8.GetStop());
        }
        [Fact(Timeout=1500)]
        public void HardStopCommandWithoutAction()
        {
            var th6 = IoC.Resolve<MyThread>("CreateAll", "90");
            var hardStopCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HardStop", "90");
            Assert.NotNull(hardStopCommand);
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "90");
            var mre1 = new ManualResetEvent(false);
            IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, new ActionCommand(() => { 
                hardStopCommand.Execute();
                mre1.Set(); }
            )).Execute();
            mre1.WaitOne(200);
            Assert.True(th6.QueueIsEmpty());
            Assert.True(th6.GetStop());
        }
    }
}
