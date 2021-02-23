using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.GameManager
{
    public class GameManagerService : BackgroundService
    {
        private readonly ILogger<GameManagerService> _logger;

        public GameManagerService(ILogger<GameManagerService> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Game Manager Started");
            return Task.CompletedTask;
        }
    }
}