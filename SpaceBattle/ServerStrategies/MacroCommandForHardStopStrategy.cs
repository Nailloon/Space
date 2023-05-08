﻿using SpaceBattle.Server;
using SpaceBattle.Interfaces;
using SpaceBattle.MacroCommand;

namespace SpaceBattle.ServerStrategies
{
    public class MacroCommandForHardStopStrategy: IStrategy
    {
        public object StartStrategy(params object[] args)
        {
            IEnumerable<Interfaces.ICommand> commands = new List<Interfaces.ICommand>();
            commands.Append(args[0]);
            commands.Append(new ActionCommand((Action)args[1]));
            return new MacroCommands(commands);
        }
    }
}
