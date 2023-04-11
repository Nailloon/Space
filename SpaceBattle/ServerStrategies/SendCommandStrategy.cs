using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;

namespace SpaceBattle.ServerStrategies
{
    public class SendCommandStrategy: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            var sendCommand = new SendCommand((ISender)args[0], (ICommand)args[1]);
            return sendCommand;
        }
    }
}
