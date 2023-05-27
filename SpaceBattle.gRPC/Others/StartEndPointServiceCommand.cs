namespace SpaceBattleGrpc.Others;

using Microsoft.AspNetCore.Builder;
using SpaceBattle.gRPC.Services;
using SpaceBattle.Interfaces;
class StartEndPointServiceCommand : ICommand
{

    WebApplication app;

    public StartEndPointServiceCommand()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddGrpc();

        this.app = builder.Build();

        app.MapGrpcService<EndPointService>();
    }

    public void Execute() => this.app.Run();
}
