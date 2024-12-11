namespace SpaceBattle.gRPC.Router;

    public interface IRouter
    {
        public bool route(string gameId, Google.Protobuf.Collections.MapField<string, string> orderMap);
    }
