using Hwdtech;
using SpaceBattle.Interfaces;

namespace SpaceBattle.Server
{
    public class UObject : IUObject
    {
        public UObject(Dictionary<string, ITransactionStatus> properties){
            this.properties = properties;
        }
        Dictionary<string, ITransactionStatus> properties;
        public object get_property(string key)
        {
            if (properties.ContainsKey(key))
            {
                if (properties[key].is_not_equal_values()){
                    if (IoC.Resolve<bool>("TransactionManager.GetTransactionStatus", properties[key].get_id())){
                        properties[key].commited();
                    }
                    else
                    {
                        properties[key].aborted();
                    }
                }
                return properties[key].get_value();
            }
            else{
                throw new KeyNotFoundException();
            }
        }

        public void set_property(string key, object value)
        {
            if (properties.ContainsKey(key)){
                if (IoC.Resolve<bool>("TransactionManager.GetTransactionStatus", properties[key].get_id())){
                        properties[key].commited();
                    }
                properties[key].set_transaction_value(value);
                properties[key].set_id(IoC.Resolve<int>("TransactionManager.GetCurrentTransactionID"));
            }
            else
            {
                properties.Add(key, new TransactionStatus(new object(), value, IoC.Resolve<int>("TransactionManager.GetCurrentTransactionID")));
            }
        }
    }
}
