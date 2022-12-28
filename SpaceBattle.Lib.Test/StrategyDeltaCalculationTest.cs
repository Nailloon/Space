using Moq;
using SpaceBattle.Collision;
using SpaceBattle.Interfaces;

namespace SpaceBattle.Lib.Test
{
    public class StrategyDeltaCalculationTest
    {
        [Fact]
        public void DeltaCalculationTest()
        {
            var UnicObject1 = new Mock<IUObject>();
            var UnicObject2 = new Mock<IUObject>();
            var DeltaStrategy = new StrategyDeltaCalculation();
            DeltaStrategy.StartStrategy(UnicObject1, UnicObject2);
        }

    }
}
