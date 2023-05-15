namespace SpaceBattle.Interfaces
{
    public interface IReceiver
    {
        public ICommand Receive();
        public bool IsEmpty();
    }
}
