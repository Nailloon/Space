using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Server
{
    public class SendCommand: SpaceBattle.Interfaces.ICommand
    {
        private ISender sndr;
        private ICommand cmd;
        public SendCommand(ISender sndr, ICommand cmd)
        {
            this.sndr = sndr;
            this.cmd = cmd;
        }
        public void Execute()
        {
            sndr.Send(cmd);
        }
    }
}
