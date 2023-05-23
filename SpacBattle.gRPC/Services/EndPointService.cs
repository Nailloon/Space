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
            IDictionary<string, string> argv = new Dictionary<string, string>();
            request.Args.Select(arg => argv[arg.GameItemId] = arg.CommandType);
            List<ICommand> commands = new List<ICommand>();
            foreach (KeyValuePair<string, string> item in argv)
            {
                commands.Add(IoC.Resolve<ICommand>("CreateCommand", item.Key, item.Value));
            }
            IoC.Resolve<ICommand>("SendCommand", gameId, new MacroCommands(commands)).Execute();
            return Task.FromResult(new CommandReply
            {
                Status = 202
            });
        }
    }
}
