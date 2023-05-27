using Grpc.Core;
using Hwdtech;
using SpaceBattle.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
            var cmd = IoC.Resolve<ICommand>("CreateCommandByNameForObject", request);
            var threadID = IoC.Resolve<string>("Storage.GetThreadByGameID", gameId);
            IoC.Resolve<ICommand>("SendCommandByThreadID", threadID, cmd).Execute();
            return Task.FromResult(new CommandReply
            {
                Status = 202
            });
        }
    }
}
