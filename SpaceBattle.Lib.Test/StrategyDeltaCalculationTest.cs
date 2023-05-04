using Moq;
using SpaceBattle.Collision;
using SpaceBattle.Interfaces;
using SpaceBattle.Auxiliary;
using Hwdtech;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test
{
    public class StrategyDeltaCalculationTest
    {
        public StrategyDeltaCalculationTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }
        [Fact]
        public void DeltaCalculationTest()
        {
            var UnicObject1 = new Mock<IUObject>();
            var UnicObject2 = new Mock<IUObject>();
            UnicObject1.Setup(x => x.get_property("Velocity")).Returns(new Vector(5, 2));
            UnicObject2.Setup(x => x.get_property("Velocity")).Returns(new Vector(0, 1));
            UnicObject1.Setup(x => x.get_property("Coords")).Returns(new Vector(1, 1));
            UnicObject2.Setup(x => x.get_property("Coords")).Returns(new Vector(0, 1));
            
            var DeltaStrategy = new StrategyDeltaCalculation();
            var GetPropertyStrategy = new StrategyGetProperty();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "UObject.GetProperty", (object[] args) => GetPropertyStrategy.StartStrategy(args)).Execute();
            Assert.True(new Vector(1, 0, 5, 1) == (Vector)DeltaStrategy.StartStrategy(UnicObject1.Object, UnicObject2.Object));
        }

    }
}
