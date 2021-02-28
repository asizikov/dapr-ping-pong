using System.Net.Http.Json;
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
        private readonly IGameManagerLoop _gameManagerLoop;

        public GameManagerService(ILogger<GameManagerService> logger, DaprClient daprClient, IGameManagerLoop gameManagerLoop)
        {
            _logger = logger;
            _daprClient = daprClient;
            _gameManagerLoop = gameManagerLoop;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Game Manager starting new game");
            var gameState = await _daprClient.GetStateAsync<GameState>("game-store", "game-state", ConsistencyMode.Strong, cancellationToken: stoppingToken);
            if (gameState is null)
            {
                gameState = new GameState();
            }
            gameState.Current = State.NewGame;

            await _daprClient.SaveStateAsync("game-store", "game-state", gameState, cancellationToken: stoppingToken);

            var httpClient = DaprClient.CreateInvokeHttpClient("cloud-eng-pingpong-player-a");
            var res = await httpClient.PostAsJsonAsync("/api/v1/stop", 1);

            while (!stoppingToken.IsCancellationRequested)
            {
                await _gameManagerLoop.ExecuteAsync(stoppingToken);
                
                await Task.Delay(1000, stoppingToken);
                await httpClient.PostAsJsonAsync("/api/v1/start", 1);
            }
        }
    }

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