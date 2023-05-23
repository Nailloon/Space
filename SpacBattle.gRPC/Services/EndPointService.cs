using Grpc.Core;
using SpaceBattle;
using Hwdtech;
using SpaceBattle.Interfaces;
using ICommand = SpaceBattle.Interfaces.ICommand;

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
            string fireCommand = request.CommandType;
            string typeOfCommand = request.CommandType;
            return Task.FromResult(new CommandReply
            {
                Status = 202
            });
        }
    }
}
