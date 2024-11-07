using SpaceBattle.Interfaces;

namespace SpaceBattle.Server
{
    public class TransactionStatus : ITransactionStatus
    {
        public TransactionStatus(object value, object transactionValue, int id){
            this.value = value;
            this.transactionValue = transactionValue;
            this.id = id;
        }

        private int id;
        private object transactionValue;
        private object value;

        public int get_id()
        {
            return id;
        }
        public void set_id(int id){
            this.id=id;
        }
        public void set_transaction_value(object newValue){
            this.transactionValue=newValue;
        }

        public object get_transaction_value()
        {
            return transactionValue;
        }

        public object get_value()
        {
            return value;
        }
        public bool is_not_equal_values()
        {
            return !transactionValue.Equals(value);
        }

        public void commited(){
            value = transactionValue;
        }
        public void aborted(){
            transactionValue = value;
        }
    }
}
