namespace SpaceBattleGrpc.Strategies;

using Microsoft.AspNetCore.Builder;
using SpaceBattle.gRPC.Services;
using SpaceBattle.Interfaces;
class StartEndPointServiceCommand : ICommand
{

    WebApplication app;

    public StartEndPointServiceCommand(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddGrpc();

        this.app = builder.Build();

        app.MapGrpcService<EndPointService>();
    }

    public void Execute() => this.app.Run();
}
