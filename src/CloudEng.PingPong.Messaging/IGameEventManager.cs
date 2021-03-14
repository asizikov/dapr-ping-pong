using System.Threading;
using System.Threading.Tasks;

namespace CloudEng.PingPong.Messaging
{
    public interface IGameEventManager
    {
        Task PublishGameEventAsync<TEvent>(TEvent data, CancellationToken token);
    }
    
    
}