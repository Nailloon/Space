using Hwdtech;
using SpaceBattle.Interfaces;

namespace SpaceBattle.Lib;

public class StartMoveCommand : Interfaces.ICommand
{
    IMoveCommandStartable installator { get; }

    public StartMoveCommand(IMoveCommandStartable UnicObj)
    {
        installator = UnicObj;
    }

    public void Execute()
    {
        installator.action.ToList().ForEach(o => IoC.Resolve<Interfaces.ICommand>("General.SetProperty", installator.Uobj, o.Key, o.Value).Execute());
        Interfaces.ICommand MCommand = IoC.Resolve<Interfaces.ICommand>("Command.Move", installator.Uobj);
        IoC.Resolve<Interfaces.ICommand>("General.SetProperty", installator.Uobj, "Commands.Movement", MCommand).Execute();
        IoC.Resolve<Interfaces.ICommand>("Queue.Push", MCommand).Execute();
    }
}
