using System.Threading;
using System.Threading.Tasks;

namespace CloudEng.PingPong.GameManager
{
    public interface IGameManagerLoop
    {
        Task ExecuteAsync(CancellationToken token);
    }
}