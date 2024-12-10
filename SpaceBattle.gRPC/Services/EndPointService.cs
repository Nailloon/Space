using Grpc.Core;
using Hwdtech;
using ICommand = SpaceBattle.Interfaces.ICommand;

namespace SpaceBattle.gRPC.Services
{
    public class EndPointService : EndPoint.EndPointBase
    {
        private readonly ILogger<EndPointService> _logger;
        private IRouter _router;
        public EndPointService(ILogger<EndPointService> logger, IRouter router)
        {
            _logger = logger;
            _router = router;
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
        
        public override Task<OrderReply> Order(OrderRequest request, ServerCallContext context)
        {
            bool isRouted = _router.route(request.GameId, request.Map);
            return Task.FromResult(new OrderReply
            {
                Status = isRouted
            });
        }
    }
}
