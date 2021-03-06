using System;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Messaging;
using CloudEng.PingPong.Player.Controllers;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.Player
{
    public class PlayerLoop : IPlayerLoop
    {
        private readonly ILogger<PlayerLoop> _logger;
        private readonly DaprClient _daprClient;
        private readonly IPlayerStateManager _playerStateManager;
        private readonly IConfiguration _configuration;
        private readonly IPlayersLuck _playersLuck;
        private readonly string _opponentName;

        public PlayerLoop(ILogger<PlayerLoop> logger, DaprClient daprClient, IPlayerStateManager playerStateManager,
            IConfiguration configuration, IPlayersLuck playersLuck)
        {
            _logger = logger;
            _daprClient = daprClient;
            _playerStateManager = playerStateManager;
            _configuration = configuration;
            _playersLuck = playersLuck;
            _opponentName = _configuration.GetValue<string>("OpponentName");
        }

        public async Task ExecuteIterationAsync(string playerName, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Player {playerName} loop iteration started");
            var playerState = await _playerStateManager.GetStateAsync(cancellationToken).ConfigureAwait(false);

            switch (playerState.Current)
            {
                case State.Ready:
                    _logger.LogInformation($"Player {playerName} is ready, waiting for any actions");
                    break;
                case State.MyTurn:
                    playerState.Counter++;
                    playerState.Current = State.Ready;
                    await _daprClient.SaveStateAsync("player-state-store", $"{playerName}-state", playerState,
                        cancellationToken: cancellationToken);

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
                            AddressedTo = _opponentName
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