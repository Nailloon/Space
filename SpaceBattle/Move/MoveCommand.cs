using SpaceBattle.Interfaces;

namespace SpaceBattle.Move
{
    public class MoveCommand : ICommand
    {
        private IMovable movable;
        public MoveCommand(IMovable movable)
        {
            this.movable = movable;
        }

        public void Execute()
        {
            movable.Position += movable.Velocity;
        }
    }
}
