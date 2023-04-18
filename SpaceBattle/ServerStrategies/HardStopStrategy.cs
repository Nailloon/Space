using Hwdtech;
using SpaceBattle.Interfaces;
using SpaceBattle.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.ServerStrategies
{
    public class HardStopStrategy : IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            Action<MyThread> HStop = (MyThread MT) => { MT.Stop(); };
            return IoC.Resolve<ICommand>("SendCommand", args[1], new HardStopCommand(args[0], HStop, args[2]));
        }
    }
}
