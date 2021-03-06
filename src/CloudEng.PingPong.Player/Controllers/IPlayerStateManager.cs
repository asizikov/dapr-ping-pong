using System.Threading;
using System.Threading.Tasks;

namespace CloudEng.PingPong.Player.Controllers
{
    public interface IPlayerStateManager
    {
        Task SetStateAsync(State state, CancellationToken cancellationToken);
        Task<PlayerState> GetStateAsync(CancellationToken cancellationToken);
    }
}