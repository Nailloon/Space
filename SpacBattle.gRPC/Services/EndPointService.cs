using Grpc.Core;
using SpaceBattle;
using Hwdtech;
using SpaceBattle.Interfaces;
using ICommand = SpaceBattle.Interfaces.ICommand;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Collections.Generic;
using SpaceBattle.MacroCommand;

namespace SpaceBattle.gRPC.Services
{
    public class EndPointService : EndPoint.EndPointBase
    {
        private readonly ILogger<EndPointService> _logger;
        public EndPointService(ILogger<EndPointService> logger)
        {
            _logger = logger;
        }

        public override Task<CommandReply> Command(CommandRequest request, ServerCallContext context)
        {
            string gameId = request.GameId;
            string CommandName = request.CommandType;
            string GameItemId = request.GameItemId;
            IDictionary<string, string> argv = new Dictionary<string, string>();
            request.Args.Select(arg => argv[arg.Key] = arg.Value).ToArray();
            var cmd = IoC.Resolve<ICommand>("CreateCommandByNameForObject", CommandName, GameItemId, argv.Keys, argv.Values);
            var sender = IoC.Resolve<ISender>("SenderAdapterGetByID", IoC.Resolve<string>("Storage.GetThreadByGameID", gameId));
            IoC.Resolve<ICommand>("SendCommand", sender, cmd).Execute();
            return Task.FromResult(new CommandReply
            {
                Status = 202
            });
        }
    }
}
