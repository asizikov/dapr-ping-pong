using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.GameManager
{
    public class GameManagerLoop : IGameManagerLoop
    {
        private readonly ILogger<GameManagerLoop> _logger;

        public GameManagerLoop(ILogger<GameManagerLoop> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(CancellationToken token)
        {
            _logger.LogInformation("Game manager loop iteration");
            return Task.CompletedTask;
        }
    }
}