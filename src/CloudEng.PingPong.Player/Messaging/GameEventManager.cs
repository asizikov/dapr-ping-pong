using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Messaging;
using Dapr.Client;

namespace CloudEng.PingPong.Player.Messaging
{
    public class GameEventManager : IGameEventManager
    {
        private readonly DaprClient _daprClient;

        public GameEventManager(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public Task PublishGameEventAsync<TEvent>(TEvent data, CancellationToken cancellationToken) 
            => _daprClient.PublishEventAsync(PubSub.GameMessaging, Topics.Game, data, cancellationToken);
    }
}