namespace SpaceBattle.Interfaces
{
    public interface IMoveCommandStartable
    {
        IUObject Uobj{ get; }
        IDictionary<string, object> operation{ get; }
    }
}
