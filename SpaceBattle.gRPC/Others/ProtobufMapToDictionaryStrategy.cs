namespace SpaceBattle.gRPC.Others;

using SpaceBattle.Interfaces;

public class ProtobufMapToDictionaryStrategy : IStrategy
{
    public object StartStrategy(params object[] args)
    {
        var map = (Google.Protobuf.Collections.MapField<string,string>)args[0];
        var properties = new List<KeyValuePair<string, string>>();
        foreach (var i in map){
            var property = new KeyValuePair<string, string>(i.Key, i.Value); 
            properties.Add(property);
        };
        return new Dictionary<string, string>(properties);
    }
}