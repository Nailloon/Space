using Hwdtech;
using SpaceBattle.Interfaces;
using System.Reflection;

namespace SpaceBattle.AdapterGenerating
{
    public class AdapterGeneratorStrategy: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var dtype = (Type)args[0];
            var dtype1 = (Type)args[1];
            var builder = IoC.Resolve<IBuilder>("IninitialAdapterCode", "Dtype", dtype, dtype1);
            var properties = dtype.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                builder.addMembers("Property", property);
            }
            return builder.Build();
        }
    }
}
