using SpaceBattle.Interfaces;
using SpaceBattle.Auxiliary;

namespace SpaceBattle.Rotate
{
    public class RotateCommand : ICommand
    {
        private IRotate rotatable;
        public RotateCommand(IRotate rotatable)
        {
            this.rotatable = rotatable;
        }
        public void Execute()
        {
            rotatable.Angle = Fraction.Summa(rotatable.Angle, rotatable.AngleVelocity);
        }
    }
}
