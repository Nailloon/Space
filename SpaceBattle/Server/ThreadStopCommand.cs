using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Server
{
    public class ThreadStopCommand: SpaceBattle.Interfaces.ICommand
    {
        MyThread stoppingThread;
        public ThreadStopCommand(MyThread stoppingThread) => this.stoppingThread = stoppingThread;
        public void Execute()
        {
            if (Thread.CurrentThread == stoppingThread.thread)
            {
                stoppingThread.Stop();
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
