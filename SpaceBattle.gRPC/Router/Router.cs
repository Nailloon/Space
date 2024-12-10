using SpaceBattle.gRPC.Others;
using SpaceBattle.Interfaces;

namespace SpaceBattle.gRPC
{
    public class Router : IRouter
    {
        Dictionary<string, string> _threadIdByGameIdDictionary;
        Dictionary<string, ISender> _senderByThreadIdDictionary;
        public Router(Dictionary<string, string> threadIdByGameIdDictionary, Dictionary<string, ISender> senderByThreadIdDictionary){
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
    }
}
