using Hwdtech;
using SpaceBattle.Auxiliary;
using SpaceBattle.Interfaces;
namespace SpaceBattle.Collision
{
    public class CheckCollision : Interfaces.ICommand
    {
        IUObject UO1 { get; }
        IUObject UO2 { get; }
        public CheckCollision(IUObject UnicObj1, IUObject UnicObj2)
        {
            UO1 = UnicObj1;
            UO2 = UnicObj2;
        }

        public void Execute()
        {
            Vector vec = IoC.Resolve<Vector>("Calculate.Delta", UO1, UO2);
            bool collision = IoC.Resolve<bool>("CollisionDecisionTree", vec);
            if (collision)
            {
                throw new Exception();
            }
        }
    }
}
