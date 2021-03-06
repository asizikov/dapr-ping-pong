using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Messaging;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.GameManager
{
    public class GameManagerLoop : IGameManagerLoop
    {
        private readonly ILogger<GameManagerLoop> _logger;
        private readonly DaprClient _daprClient;

        public GameManagerLoop(ILogger<GameManagerLoop> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            _logger.LogInformation("Game manager loop iteration");
            var gameState = await _daprClient.GetStateAsync<GameState>("game-store", "game-state",
                ConsistencyMode.Strong, cancellationToken: token);
            switch (gameState.Current)
            {
                case State.NewGame:
                    gameState.Current = State.Started;
                    await _daprClient.SaveStateAsync("game-store", "game-state", gameState, cancellationToken: token);
                    var player = "player-b";
                    if (gameState.PlayerA <= gameState.PlayerB)
                    {
                        player = "player-a";
                    }

                    await _daprClient.PublishEventAsync(PubSub.GameMessaging, Topics.GameCommands,
                        new GameControlEvent
                        {
                            AddressedTo = player,
                            Command = GameCommand.Start
                        }, token).ConfigureAwait(false);
                    break;
                default:
                    _logger.LogInformation("Current game state is {GameState}. Skipping this iteration",
                        gameState.Current);
                    break;
            }
        }
    }
}