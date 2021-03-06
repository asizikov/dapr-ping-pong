using System;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Messaging;
using Dapr;
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
        private readonly IPlayerStateManager _playerStateManager;
        private readonly IConfiguration _configuration;
        private readonly string _playerName;

        public GameController(ILogger<GameController> logger, IPlayerStateManager playerStateManager, IConfiguration configuration)
        {
            _logger = logger;
            _playerStateManager = playerStateManager;
            _configuration = configuration;
            _playerName = _configuration.GetValue<string>("PlayerName");
        }

        [HttpPost("gameEvent")]
        [Topic(PubSub.GameMessaging, Topics.GameCommands)]
        public async Task<IActionResult> StartAsync(GameControlEvent gameEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received {Command}", gameEvent.Command);
            switch (gameEvent.Command)
            {
                case GameCommand.Start:
                    if (gameEvent.AddressedTo == _playerName)
                    {
                        _logger.LogInformation("Starting the game");
                        await _playerStateManager.SetStateAsync(State.MyTurn, cancellationToken).ConfigureAwait(false);
                    }

                    break;
                case GameCommand.Stop:
                    await _playerStateManager.SetStateAsync(State.Ready, cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new OkResult();
        }

        [HttpPost("ping")]
        [Topic(PubSub.GameMessaging, Topics.Game)]
        public async Task<IActionResult> PingAsync(GameProcessEvent gameProcessEvent,
            CancellationToken cancellationToken)
        {
            if (gameProcessEvent.AddressedTo == _playerName)
            {
                _logger.LogInformation("Player {PlayerName} Got a ping {Ping}", _playerName, gameProcessEvent.Ping);
                await _playerStateManager.SetStateAsync(State.MyTurn, cancellationToken);
            }

            return new OkResult();
        }
    }
}