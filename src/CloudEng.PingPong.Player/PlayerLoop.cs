using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.Player
{
    public class PlayerLoop : IPlayerLoop
    {
        private readonly ILogger<PlayerLoop> _logger;
        private readonly DaprClient _daprClient;

        public PlayerLoop(ILogger<PlayerLoop> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        public async Task ExecuteIterationAsync(string playerName, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Player {playerName} loop iteration started");
            var playerState = await _daprClient.GetStateAsync<PlayerState>("player-state-store", $"{playerName}-state", ConsistencyMode.Strong, cancellationToken: cancellationToken);
            switch (playerState.Current)
            {
                case State.Ready:
                    _logger.LogInformation($"Player {playerName} is ready, waiting for any actions");
                    break;
                case State.MyTurn:
                    _logger.LogInformation($"Player {playerName} is taken turn, ready to ping");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}