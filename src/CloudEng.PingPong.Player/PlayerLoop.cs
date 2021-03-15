using System;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Messaging;
using CloudEng.PingPong.Player.Configuration;
using CloudEng.PingPong.Player.Controllers;
using CloudEng.PingPong.Player.Messaging;
using CloudEng.PingPong.Player.StateManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloudEng.PingPong.Player
{
    public class PlayerLoop : IPlayerLoop
    {
        private readonly ILogger<PlayerLoop> _logger;
        private readonly IGameEventManager _gameEventManager;
        private readonly IPlayerStateManager _playerStateManager;
        private readonly IOptions<PlayerConfigOptions> _configuration;
        private readonly IPlayersLuck _playersLuck;

        public PlayerLoop(ILogger<PlayerLoop> logger, IGameEventManager gameEventManager,
            IPlayerStateManager playerStateManager,
            IOptions<PlayerConfigOptions> configuration, IPlayersLuck playersLuck)
        {
            _logger = logger;
            _gameEventManager = gameEventManager;
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
                    _logger.LogInformation("Player {PlayerName} is ready, waiting for any actions",
                        _configuration.Value.PlayerName);
                    break;
                case State.MyTurn:
                    await _playerStateManager.SetStateAsync(State.Ready, playerState.Counter + 1, cancellationToken)
                        .ConfigureAwait(false);

                    if (_playersLuck.ShouldMissCurrentTake(playerState.Counter))
                    {
                        _logger.LogWarning("Player {PlayerName} missed their turn", playerName);
                        await _gameEventManager.PublishGameEventAsync(new PlayerLostEvent
                        {
                            PlayerName = playerName
                        }, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await _gameEventManager.PublishGameEventAsync(new GameProcessEvent
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