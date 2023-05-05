﻿using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.Server
{
    public class ThreadStopCommand: ICommand
    {
        MyThread stoppingThread;
        Action action;
        public ThreadStopCommand(MyThread stoppingThread, Action? action=null)
        {
            this.stoppingThread = stoppingThread;
            this.action = action;
        }
        public void Execute()
        {
            if (Thread.CurrentThread == stoppingThread.GetThread())
            {
                stoppingThread.Stop();
                action?.Invoke();
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
