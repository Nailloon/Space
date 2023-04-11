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
        public SendCommand(ISender sndr, ICommand cmd)
        {
            
        }
        public void Execute()
        {
            sndr.Send(cmd);
        }
    }
}
