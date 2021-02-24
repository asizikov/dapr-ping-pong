using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.Player
{
    public class PlayerManager : BackgroundService
    {
        private readonly ILogger<PlayerManager> _logger;

        public PlayerManager(ILogger<PlayerManager> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Player started");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Ping");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}