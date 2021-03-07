using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Messaging;
using Dapr;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.GameManager.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly DaprClient _daprClient;

        public GameController(ILogger<GameController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [HttpPost("player-lost")]
        [Topic(PubSub.GameMessaging, Topics.Game)]
        public async Task<IActionResult> PingAsync(PlayerLostEvent gameProcessEvent,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Player {PlayerName} lost current take ", gameProcessEvent.PlayerName);

            var gameState = await _daprClient.GetStateAsync<GameState>("game-store", "game-state",
                ConsistencyMode.Strong, cancellationToken: cancellationToken).ConfigureAwait(false);
            gameState.Current = State.NewGame;
            
            await _daprClient.SaveStateAsync("game-store", "game-state", gameState, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return new OkResult();
        }
    }
}