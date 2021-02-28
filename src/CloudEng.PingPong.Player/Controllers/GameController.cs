using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.Player.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly DaprClient _daprClient;
        private readonly IConfiguration _configuration;
        private readonly string _playerName;
        public GameController(ILogger<GameController> logger, DaprClient daprClient, IConfiguration configuration)
        {
            _logger = logger;
            _daprClient = daprClient;
            _configuration = configuration;
            _playerName = _configuration.GetValue<string>("PlayerName");
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");
            await SetStateAsync(State.MyTurn, cancellationToken);
            return new OkResult();
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");
            await SetStateAsync(State.Ready, cancellationToken);
            return new OkResult();
        }

        [HttpPost("ping")]
        public async Task<IActionResult> PingAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pinging");
            return new OkResult();
        }


        private async Task SetStateAsync(State state, CancellationToken cancellationToken)
        {
            var playerState = await _daprClient.GetStateAsync<PlayerState>("player-state-store", $"{_playerName}-state", ConsistencyMode.Strong, cancellationToken: cancellationToken);
            playerState.Current = state;
            await _daprClient.SaveStateAsync("player-state-store", $"{_playerName}-state", playerState, cancellationToken: cancellationToken);
        }
    }
}