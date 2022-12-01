using SpaceBattle.Auxiliary;

namespace SpaceBattle.Move
{
    public interface IMovable
    {
        Vector Position { get; set; }
        Vector Velocity { get; }
    }
}
