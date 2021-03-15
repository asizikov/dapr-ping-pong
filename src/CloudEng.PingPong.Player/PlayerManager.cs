using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Player.StateManagement;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.Player
{
    public class PlayerManager : BackgroundService
    {
        private readonly ILogger<PlayerManager> _logger;
        private readonly IPlayerLoop _playerLoop;
        private readonly DaprClient _daprClient;
        private readonly IConfiguration _configuration;

        public PlayerManager(ILogger<PlayerManager> logger, IPlayerLoop playerLoop, DaprClient daprClient,
            IConfiguration configuration)
        {
            _logger = logger;
            _playerLoop = playerLoop;
            _daprClient = daprClient;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var playerName = _configuration.GetValue<string>("PlayerName");
            _logger.LogInformation("Player started");
            await _daprClient.SaveStateAsync("player-state-store", $"{playerName}-state",
                new PlayerState {Current = State.Ready},
                cancellationToken: stoppingToken).ConfigureAwait(false);
            while (!stoppingToken.IsCancellationRequested)
            {
                await _playerLoop.ExecuteIterationAsync(playerName, stoppingToken).ConfigureAwait(false);
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}