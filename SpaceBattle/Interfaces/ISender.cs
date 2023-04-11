namespace SpaceBattle.Interfaces
{
    public interface ISender
    {
        public void Send(ICommand command);
    }
}
