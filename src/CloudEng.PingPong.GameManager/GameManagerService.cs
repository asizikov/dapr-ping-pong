using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.GameManager
{
    public class GameManagerService : BackgroundService
    {
        private readonly ILogger<GameManagerService> _logger;
        private readonly DaprClient _daprClient;

        public GameManagerService(ILogger<GameManagerService> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Game Manager Started");
            while (!stoppingToken.IsCancellationRequested)
            {
                var gameScore = await _daprClient.GetStateAsync<GameScore>("game-store","score");
                _logger.LogInformation($"Current Score {gameScore}");
                //await _daprClient.InvokeMethodAsync<object>("cloud-eng-pingpong-player-a", "game.start", null, stoppingToken);
                await Task.Delay(1000, stoppingToken);
                gameScore.PlayerA++;
                await _daprClient.SaveStateAsync("game-store", "score", gameScore );
            }
        }
    }

    public class GameScore
    {
        public int PlayerA { get; set; } 
        public int PlayerB
        {
            get;
            set;
        }
    } 
}