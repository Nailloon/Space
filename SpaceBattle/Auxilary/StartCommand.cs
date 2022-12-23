﻿using Hwdtech;
using SpaceBattle.Interfaces;

namespace SpaceBattle.Lib;

public class StartMoveCommand : Interfaces.ICommand
{
    IMoveCommandStartable order { get; }

    public StartMoveCommand(IMoveCommandStartable UnicObj)
    {
        order = UnicObj;
    }

    public void Execute()
    {
        order.operation.ToList().ForEach(o => IoC.Resolve<Interfaces.ICommand>("Сomprehensive.SetProperty", order.Uobj, o.Key, o.Value).Execute());
        Interfaces.ICommand MCommand = IoC.Resolve<Interfaces.ICommand>("Adapt.Move", order.Uobj);
        IoC.Resolve<Interfaces.ICommand>("Сomprehensive.SetProperty", order.Uobj, "Commands.Movement", MCommand).Execute();
        IoC.Resolve<Interfaces.ICommand>("Queue.Push", MCommand).Execute();
    }
}