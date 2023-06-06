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
            var builder = IoC.Resolve<IBuilder>("BuilderForDtype1ToDtype2", dtype, dtype1);
            builder.addMembers("Dtype", dtype);
            var properties = dtype.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                builder.addMembers("Property", property);
            }
            return builder.Build();
        }
    }
}
