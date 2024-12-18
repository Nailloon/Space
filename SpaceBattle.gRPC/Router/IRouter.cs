using SpaceBattle.Interfaces;

namespace SpaceBattle.gRPC.Router;

    public interface IRouter
    {
        public bool route(string gameId, Google.Protobuf.Collections.MapField<string, string> orderMap);
        public bool routeMigrateCommand(string serverId, string gameId);
        public bool routeAcceptCommand(string serializedGame);
    }
