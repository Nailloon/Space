using Grpc.Net.Client;
using gRPC.Outbox;
using Google.Protobuf.Collections;
using Grpc.Core;
using Npgsql;

using var channel = GrpcChannel.ForAddress("http://localhost:5103");
var client = new EndPoint.EndPointClient(channel);

var messages = new List<string>();
var connString = "Host=localhost;Username=postgres;Password=postgress;Database=transactional_outbox";
await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();
var serverData = client.Order();
await using (var cmd = new NpgsqlCommand("SELECT message FROM messages WHERE is_sent=false", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    while (await reader.ReadAsync())
        messages.Add(reader.GetString(0));
}
for (int i = 0; i < messages.Count; i++)
{
    string mes = messages[i];
    Dictionary<string, string> dict = new();
    string[] props = mes.Split(';');
    for (int j = 0; j < props.Length; j++)
    {
        string[] keyValue = props[j].Split('=');
        if (keyValue.Length == 2)
            dict.Add(keyValue[0], keyValue[1]);
    }
    try
    {
        var gRPCMess = new OrderRequest();
        var propsMap = new MapField<string, string>{dict};
        gRPCMess.Map.Add(propsMap);
        gRPCMess.GameId = propsMap.First().Value;
        await serverData.RequestStream.WriteAsync(gRPCMess);
    }
    catch
    {
        continue;
    }
}
await serverData.RequestStream.CompleteAsync();

var replies = serverData.ResponseStream.ReadAllAsync();
int num = 0;
await foreach (OrderReply reply in replies)
{
    Console.WriteLine("OrderStatus: " + reply.Status);
    await using (var cmd = new NpgsqlCommand("UPDATE messages SET is_sent=true WHERE \"message\"=(@p)", conn))
    {
        Console.WriteLine(messages[num]);
        cmd.Parameters.AddWithValue("p", messages[num]);
        await cmd.ExecuteNonQueryAsync();
    }
    num+=1;
}
