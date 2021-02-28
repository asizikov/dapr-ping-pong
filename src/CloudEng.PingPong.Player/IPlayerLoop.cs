using System.Threading;
using System.Threading.Tasks;

namespace CloudEng.PingPong.Player
{
    public interface IPlayerLoop
    {
        Task ExecuteIterationAsync(string playerName, CancellationToken cancellationToken);
    }
}