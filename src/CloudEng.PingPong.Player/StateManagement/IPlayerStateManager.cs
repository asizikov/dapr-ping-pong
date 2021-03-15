using System.Threading;
using System.Threading.Tasks;

namespace CloudEng.PingPong.Player.StateManagement
{
    public interface IPlayerStateManager
    {
        Task SetStateAsync(State state, CancellationToken cancellationToken);
        Task SetStateAsync(State state, int counter, CancellationToken cancellationToken);
        Task<PlayerState> GetStateAsync(CancellationToken cancellationToken);
    }
}