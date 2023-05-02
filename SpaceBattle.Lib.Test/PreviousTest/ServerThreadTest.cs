using Hwdtech.Ioc;
using Hwdtech;
using Moq;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Lib.Test
{
    public class ServerThreadTest
    {
        [Fact]

        public void MyThread_queue()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            BlockingCollection<SpaceBattle.Interfaces.ICommand> commands = new BlockingCollection<SpaceBattle.Interfaces.ICommand>(100);

            var receiver = new Mock<IReceiver>();
            receiver.Setup(r => r.Receive()).Returns(() => commands.Take());
            receiver.Setup(r => r.IsEmpty()).Returns(()=> commands.Count == 0);
            var sender = new Mock<ISender>();
            sender.Setup(s => s.Send(It.IsAny<SpaceBattle.Interfaces.ICommand>())).Callback<SpaceBattle.Interfaces.ICommand>((c)=> commands.Add(c));
            Assert.True(receiver.Object.IsEmpty());
            var cmd1 = new ActionCommand(
                () =>
                {
                    Thread.Sleep(1000);
                });
            var cmd2 = new ActionCommand(
                () =>
                {
                    Thread.Sleep(1);
                });
            var cmd3 = new ActionCommand(
                () =>
                {
                    mre.Set();
                    //barrier.SignalAndWait(1)
                });
            sender.Object.Send(cmd1);
            sender.Object.Send(cmd2);
            sender.Object.Send(cmd3);
            Assert.False(receiver.Object.IsEmpty());

            MyThread st = new MyThread(receiver.Object);
            st.Execute();

            //Thread.Sleep(1000);
            mre.WaitOne();

            //Assert.Equal(0, queue.Count);
            Assert.True(receiver.Object.IsEmpty());
        }

        [Fact]

        public void MyThread_2()
        {
            Barrier barrier = new Barrier(3);

            BlockingCollection<SpaceBattle.Interfaces.ICommand> commands1 = new BlockingCollection<SpaceBattle.Interfaces.ICommand>();
            BlockingCollection<SpaceBattle.Interfaces.ICommand> commands2 = new BlockingCollection<SpaceBattle.Interfaces.ICommand>();
            BlockingCollection<SpaceBattle.Interfaces.ICommand> commands3 = new BlockingCollection<SpaceBattle.Interfaces.ICommand>();

            var receiver1 = new Mock<IReceiver>();
            receiver1.Setup(r => r.Receive()).Returns(() => commands1.Take());
            receiver1.Setup(r => r.IsEmpty()).Returns(() => commands1.Count == 0);
            var receiver2 = new Mock<IReceiver>();
            receiver2.Setup(r => r.Receive()).Returns(() => commands2.Take());
            receiver2.Setup(r => r.IsEmpty()).Returns(() => commands2.Count == 0);

            Assert.True(receiver1.Object.IsEmpty());
            Assert.True(receiver2.Object.IsEmpty());

            var sender1 = new Mock<ISender>();
            sender1.Setup(s => s.Send(It.IsAny<SpaceBattle.Interfaces.ICommand>())).Callback<SpaceBattle.Interfaces.ICommand>((c) => commands1.Add(c));
            var sender2 = new Mock<ISender>();
            sender2.Setup(s => s.Send(It.IsAny<SpaceBattle.Interfaces.ICommand>())).Callback<SpaceBattle.Interfaces.ICommand>((c) => commands2.Add(c));

            var cmd1 = new ActionCommand(
                () =>
                {
                    Thread.Sleep(1000);
                });
            var cmd2 = new ActionCommand(
                ()=>
                {
                    Thread.Sleep(1);
                });
            var cmd3 = new ActionCommand(
                () =>
                {
                    barrier.SignalAndWait(1);
                }
                );
            var cmd4 = new ActionCommand(
                () =>
                {
                    barrier.SignalAndWait(1);
                });

            sender1.Object.Send(cmd1);
            sender1.Object.Send(cmd2);
            sender1.Object.Send(cmd3);

            sender2.Object.Send(cmd2);
            sender2.Object.Send(cmd1);
            sender2.Object.Send(cmd4);

            Assert.False(receiver1.Object.IsEmpty());
            MyThread myt1 = new MyThread(receiver1.Object);
            myt1.Execute();

            MyThread myt2 = new MyThread(receiver1.Object);
            myt2.Execute();

            barrier.SignalAndWait(1);

            Assert.True(receiver1.Object.IsEmpty());
            Assert.True(receiver2.Object.IsEmpty());
        }

        [Fact]
        public void MyThread_StrategyCreateAndRun()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            var CrAStStrategy = new CreateAndStartStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateAndStartThread", (object[] args) => CrAStStrategy.StartStrategy(args)).Execute();
            var Th1 = IoC.Resolve<MyThread>("CreateAndStartThread", "83675");
            var Th2 = IoC.Resolve<MyThread>("CreateAndStartThread", "83675", (() => Thread.Sleep(5000)));
            Assert.False(Th1 == Th2);
            Assert.False(Th1.Equals(Th2));
        }

    }
}
