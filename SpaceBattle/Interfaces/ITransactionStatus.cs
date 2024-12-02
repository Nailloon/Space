namespace SpaceBattle.Interfaces
{
    public interface ITransactionStatus
    {
        public object get_value();
        public void set_transaction_value(object newValue);
        public int get_id();
        public void set_id(int id);
        public bool is_not_equal_values();
        public void commited();
        public void aborted();
    }
}
