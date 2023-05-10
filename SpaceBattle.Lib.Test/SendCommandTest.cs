using Hwdtech.Ioc;
using Hwdtech;

namespace SpaceBattle.Lib.Test
{
    public class SendCommandTest
    {
        public SendCommandTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }

    }
}
