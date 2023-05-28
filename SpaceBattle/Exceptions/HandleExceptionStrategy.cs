using ICommand = SpaceBattle.Interfaces.ICommand;
using SpaceBattle.Interfaces;
using Hwdtech;
using SpaceBattle.Server;

namespace SpaceBattle.Exceptions
{
    public class HandleExceptionStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var exception = (Exception)args[0];
            var command = (ICommand)args[1];

            var dictExceptionHandlers = IoC.Resolve<IDictionary<Exception, Dictionary<ICommand, IStrategy>>>("Dictionary.Handler.Exception");

            if (!dictExceptionHandlers.ContainsKey(exception) || !dictExceptionHandlers[exception].ContainsKey(command))
            {
                var commandData = new Dictionary<string, object>();

                commandData["NoStrategyForCommand"] = command;
                var ex = new Exception();
                ex.Data["Unknown"] = ex;
                throw ex;
            }

            else
            {
                return dictExceptionHandlers[exception][command].StartStrategy();
            }
        }
    }
}
