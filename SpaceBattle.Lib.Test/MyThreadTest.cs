using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using SpaceBattle.ServerStrategies;
using System.Collections.Concurrent;
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
        [Fact]
        public void MyThreadCreateTest()
        {
            var Th1 = IoC.Resolve<MyThread>("CreateAll", "83675");
            var Th2 = IoC.Resolve<MyThread>("CreateAll", "83675", (() => Thread.Sleep(5000)));
            Assert.NotNull(Th1);
            Assert.True(Th1.QueueIsEmpty());
            Assert.False(Th1 == Th2);
            Assert.False(Th1.Equals(Th2));

            Th1.Stop();
            Th2.Stop();
            Thread.Sleep(1000);
        }
        [Fact]
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
            Assert.True(th1.GetStop());

            th1.Stop();
            Thread.Sleep(1000);
        }
        [Fact]
        public void MyThreadHardStopTest()
        {
            var th3 = IoC.Resolve<MyThread>("CreateAll", "83673");
            var mre1 = new ManualResetEvent(false);
            Assert.True(th3.QueueIsEmpty());
            var hardStopCommand =IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HardStop", "83673", () => { mre1.Set(); });
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", "83673");
            var sendCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender, hardStopCommand);
            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th3.QueueIsEmpty());
            Assert.True(th3.GetStop());

            th3.Stop();
            Thread.Sleep(1000);
        }
        [Fact]
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


            //Barrier barrier = new Barrier(3);
       //     IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender1, IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HardStop", "83673", () => { barrier.SignalAndWait(1); })).Execute();
         //   IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender2, IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HardStop", "835", () => { barrier.SignalAndWait(1); })).Execute();
           // barrier.SignalAndWait(1);
            th3.Stop();
            th6.Stop();
            Thread.Sleep(1000);
        }
        [Fact]
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


            var th3 = IoC.Resolve<MyThread>("CreateAll", "83673", act1);
            var th6 = IoC.Resolve<MyThread>("CreateAll", "835", act1);
            var mre1 = new ManualResetEvent(false);

            var softStopCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HardStop", "835", () => { mre1.Set(); });
            var sender1 = IoC.Resolve<ISender>("SenderAdapterGetByID", "83673");
            var sender2 = IoC.Resolve<ISender>("SenderAdapterGetByID", "835");
            var sendCommand = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender1, softStopCommand);
            sendCommand.Execute();
            mre1.WaitOne(200);
            Assert.True(th3.QueueIsEmpty());
            Assert.False(th3.GetStop());
            Assert.False(th6.GetStop());
            command1.Verify();
            regStrategy1.Verify();

            //  Barrier barrier = new Barrier(3);
            // IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender1, IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HardStop", "83673", () => { barrier.SignalAndWait(1); })).Execute();
            // IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", sender2, IoC.Resolve<SpaceBattle.Interfaces.ICommand>("HardStop", "835", () => { barrier.SignalAndWait(1); })).Execute();
            // barrier.SignalAndWait(1);
            th3.Stop();
            th6.Stop();
            Thread.Sleep(1000);
        }
        //[Fact]
        //public void SenderAdapterCanSendTest()
        //{
        //    var th4 = IoC.Resolve<MyThread>("CreateAll", "8367");
        //    var th5 = IoC.Resolve<MyThread>("CreateAll", "83");
         //   var senderAdapter
         //   var command1 = new Mock<Interfaces.ICommand>();
         //   command1.Setup(_command => _command.Execute()).Verifiable();
        //    var sendCommand1 = IoC.Resolve<SpaceBattle.Interfaces.ICommand>("SendCommand", "83", command1);
       // }
    }
}
