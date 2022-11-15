using SpaceBattle.Auxiliary;

namespace SpaceBattle.Rotate
{
    public interface IRotate
    {
        Fraction Angle { get; set; }
        Fraction AngleVelocity { get; }
    }
}