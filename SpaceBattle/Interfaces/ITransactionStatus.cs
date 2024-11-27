namespace SpaceBattle.Interfaces
{
    public interface ITransactionStatus
    {
        object get_value();
        public void set_transaction_value(object newValue);
        int get_id();
        public void set_id(int id);
        bool is_not_equal_values();
        public void commited();
        public void aborted();
    }
}
