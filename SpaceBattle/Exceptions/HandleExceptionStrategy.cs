using ICommand = SpaceBattle.Interfaces.ICommand;
using SpaceBattle.Interfaces;
using Hwdtech;

namespace SpaceBattle.Exceptions
{
    public class HandleExceptionStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var exception = (Exception)args[0];
            var command = (ICommand)args[1];

            var dictExceptionHandlers = IoC.Resolve<IDictionary<Exception, IDictionary<ICommand, IStrategy>>>("Dictionary.Handler.Exception");

            if (!dictExceptionHandlers.ContainsKey(exception) || !dictExceptionHandlers[exception].ContainsKey(command))
            {
                throw new KeyNotFoundException("Handler not found for the command and exception");
            }
            else
            {
                return dictExceptionHandlers[exception][command];
            }
        }
    }
}
