using Hwdtech.Ioc;
using Hwdtech;
using SpaceBattle.AdapterGenerating;
using SpaceBattle.Move;

namespace SpaceBattle.Lib.Test
{
    public class AdapterGeneratorTest
    {
        public AdapterGeneratorTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            var adapterGenerator = new AdapterGenerator();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "BuilderForDtype1ToDtype2", (object[] args) => adapterGenerator).Execute();
        }
string exampleIMovable = @"public class IMovable_adapter : SpaceBattle.Move.IMovable {
    private System.Collections.Generic.IDictionary<string, object> target;
    public IMovable_adapter(System.Collections.Generic.IDictionary<string, object> target) {
        this.target = target;
    }
    public SpaceBattle.Auxiliary.Vector Position {
        get => IoC.Resolve<SpaceBattle.Auxiliary.Vector>(""UObjectGetValue"", target, Position);
        set => IoC.Resolve<ICommand>(""UObjectSetValue"", target, Position, propertyValue).Execute();
    }
    public SpaceBattle.Auxiliary.Vector Velocity {
        get => IoC.Resolve<SpaceBattle.Auxiliary.Vector>(""UObjectGetValue"", target, Velocity);
    }
}";
        [Fact]
        void sameStringCodeTest()
        {
            var adapterGeneratorStrategy = new AdapterBuilderStrategy();
            var template = (string)adapterGeneratorStrategy.StartStrategy(typeof(IMovable), typeof(object));
            Assert.Equal(exampleIMovable.Replace("\r\n","").Replace("    ", "").Replace("\n", ""), template);
        }
    }
}

