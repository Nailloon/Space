using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Server
{
    public class ActionCommand: ICommand
    {
        private Action action;
        public ActionCommand(Action action)
        {
            this.action = action;
        }
        public void Execute()
        {
            action();
        }
    }
}
