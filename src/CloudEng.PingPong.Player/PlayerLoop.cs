using System;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Messaging;
using CloudEng.PingPong.Player.Configuration;
using CloudEng.PingPong.Player.Controllers;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloudEng.PingPong.Player
{
    public class PlayerLoop : IPlayerLoop
    {
        private readonly ILogger<PlayerLoop> _logger;
        private readonly DaprClient _daprClient;
        private readonly IPlayerStateManager _playerStateManager;
        private readonly IOptions<PlayerConfigOptions> _configuration;
        private readonly IPlayersLuck _playersLuck;

        public PlayerLoop(ILogger<PlayerLoop> logger, DaprClient daprClient, IPlayerStateManager playerStateManager,
            IOptions<PlayerConfigOptions> configuration, IPlayersLuck playersLuck)
        {
            _logger = logger;
            _daprClient = daprClient;
            _playerStateManager = playerStateManager;
            _configuration = configuration;
            _playersLuck = playersLuck;
        }

        public async Task ExecuteIterationAsync(string playerName, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Player {PlayerName} loop iteration started", _configuration.Value.PlayerName);
            var playerState = await _playerStateManager.GetStateAsync(cancellationToken).ConfigureAwait(false);

            switch (playerState.Current)
            {
                case State.Ready:
                    _logger.LogInformation("Player {PlayerName} is ready, waiting for any actions",_configuration.Value.PlayerName);
                    break;
                case State.MyTurn:
                    await _playerStateManager.SetStateAsync(State.Ready, playerState.Counter + 1, cancellationToken).ConfigureAwait(false);
                    
                    if (_playersLuck.ShouldMissCurrentTake(playerState.Counter))
                    {
                        _logger.LogWarning("Player {PlayerName} missed their turn", playerName);
                        await _daprClient.PublishEventAsync(PubSub.GameMessaging, Topics.Game, new PlayerLostEvent
                        {
                            PlayerName = playerName
                        }, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await _daprClient.PublishEventAsync(PubSub.GameMessaging, Topics.Game, new GameProcessEvent
                        {
                            Ping = playerState.Counter,
                            AddressedTo = _configuration.Value.OpponentName
                        }, cancellationToken).ConfigureAwait(false);
                        _logger.LogInformation("Player {PlayerName} has taken their turn", playerName);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}