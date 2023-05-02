using Hwdtech.Ioc;
using Hwdtech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SpaceBattle.Server;

namespace SpaceBattle.Lib.Test
{
    public class MyThreadTest
    {
        public MyThreadTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            var threadDict = new ConcurrentDictionary<string, MyThread>();
            var senderDict = new ConcurrentDictionary<string, SenderAdapter>();
            IoC.Resolve<ICommand>("IoC.Register", "ThreadIDMyThreadMapping", () => threadDict).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "ThreadIDSenderMapping", () => senderDict).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "SenderAdapterGetByID", (string id) => senderDict[id]).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "ServerThreadGetByID", (string id) => threadDict[id]).Execute();
        }
        [Fact]
        public void MyThreadHardStopTest()
        {

        }
        public void MyThreadSoftStopTest()
        {

        }
        public void MyThreadCreateTest()
        {

        }
        public void MyThreadUpdateBehaviorTest()
        {

        }
        public void MyThreadWorkingTogetherTest()
        {

        }
    }
}
