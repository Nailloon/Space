using Grpc.Core;
using Hwdtech;
using ICommand = SpaceBattle.Interfaces.ICommand;
namespace SpaceBattle.gRPC.Services
{
    public class EndPointService : EndPoint.EndPointBase
    {
        private Router.IRouter _router;
        public EndPointService(Router.IRouter router)
        {
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
        
        public override async Task<OrderReply> Order(IAsyncStreamReader<OrderRequest> requestStream, IServerStreamWriter<OrderReply> responseStream, ServerCallContext context)
        {
           await foreach (var message in requestStream.ReadAllAsync())
            {
                bool isRouted = _router.route(message.GameId, message.Map);
                await responseStream.WriteAsync(new OrderReply(){Status = isRouted});
            }
            return new OrderReply();
        }

        public async override Task<newGameStatus> MigrateGame(gameStatus request, ServerCallContext context)
        {
            string gameId = request.GameId;
            bool isRouted = _router.routeMigrateCommand(request.NewServerId, gameId);
            return await Task.FromResult(new newGameStatus
            {
                GameStatus = isRouted
            });
        }

        public override Task<acceptStatus> AcceptGame(serializedGameMessage request, ServerCallContext context)
        {
            string serializedGame = request.SerializedGame;
            bool isRouted = _router.routeAcceptCommand(serializedGame);
            return Task.FromResult(new acceptStatus
            {
                AcceptStatus = isRouted
            });
        }
    }
}
