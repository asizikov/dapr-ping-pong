using System;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Messaging;
using Dapr;
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

        [HttpPost("gameEvent")]
        [Topic(PubSub.GameMessaging, Topics.GameCommands)]
        public async Task<IActionResult> StartAsync(GameControlEvent gameEvent,CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received {Command}", gameEvent.Command);
            switch (gameEvent.Command)
            {
                case GameCommand.Start:
                    await SetStateAsync(State.MyTurn, cancellationToken);
                    break;
                case GameCommand.Stop:
                    await SetStateAsync(State.Ready, cancellationToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
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