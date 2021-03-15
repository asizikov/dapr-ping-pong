using System.Threading;
using System.Threading.Tasks;

namespace CloudEng.PingPong.Player.Messaging
{
    public interface IGameEventManager
    {
        Task PublishGameEventAsync<TEvent>(TEvent data, CancellationToken token);
    }
}