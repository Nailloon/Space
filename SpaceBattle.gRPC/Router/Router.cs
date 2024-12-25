using System.Collections.Concurrent;
using SpaceBattle.gRPC.Others;
using SpaceBattle.Interfaces;

namespace SpaceBattle.gRPC.Router
{
    public class Router : IRouter
    {
        ConcurrentDictionary<string, string> _threadIdByGameIdDictionary;
        ConcurrentDictionary<string, ISender> _senderByThreadIdDictionary;
        Random random = new Random();
        public Router(ConcurrentDictionary<string, string> threadIdByGameIdDictionary, ConcurrentDictionary<string, ISender> senderByThreadIdDictionary){
            _threadIdByGameIdDictionary = threadIdByGameIdDictionary;
            _senderByThreadIdDictionary = senderByThreadIdDictionary;
        }
        public bool route(string gameId, Google.Protobuf.Collections.MapField<string, string> orderMap)
        {
            try
            {
                string threadId = _threadIdByGameIdDictionary[gameId];
                ISender sender = _senderByThreadIdDictionary[threadId];
                ICommand command = new BecomeCommand(gameId, orderMap);
                sender.Send(command);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool routeAcceptCommand(string serializedGame)
        {
            try
            {
                string threadId = _threadIdByGameIdDictionary.ElementAt(random.Next(0, _threadIdByGameIdDictionary.Count)).Value;
                ISender sender = _senderByThreadIdDictionary[threadId];
                ICommand command = new DeserializeCommand(threadId, serializedGame);
                sender.Send(command);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool routeMigrateCommand(string serverId, string gameId)
        {
            try
            {
                string threadId = _threadIdByGameIdDictionary[gameId];
                ISender sender = _senderByThreadIdDictionary[threadId];
                ICommand command = new SerializeCommand(gameId, serverId);
                sender.Send(command);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
