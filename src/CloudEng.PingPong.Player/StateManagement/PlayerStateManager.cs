using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Configuration;

namespace CloudEng.PingPong.Player.StateManagement
{
    public class PlayerStateManager : IPlayerStateManager
    {
        private readonly DaprClient _daprClient;
        private readonly IConfiguration _configuration;
        private readonly string _playerName;

        public PlayerStateManager(IConfiguration configuration, DaprClient daprClient)
        {
            _configuration = configuration;
            _daprClient = daprClient;
            _playerName = _configuration.GetValue<string>("PlayerName");
        }

        public async Task SetStateAsync(State state, CancellationToken cancellationToken)
        {
            var playerState = await GetStateAsync(cancellationToken).ConfigureAwait(false);

            playerState.Current = state;
            await _daprClient.SaveStateAsync("player-state-store", $"{_playerName}-state", playerState,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task SetStateAsync(State state, int counter, CancellationToken cancellationToken)
        {
            var playerState = await GetStateAsync(cancellationToken).ConfigureAwait(false);

            playerState.Current = state;
            playerState.Counter = counter;
            await _daprClient.SaveStateAsync("player-state-store", $"{_playerName}-state", playerState,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<PlayerState> GetStateAsync(CancellationToken cancellationToken)
        {
            var playerState = await _daprClient.GetStateAsync<PlayerState>("player-state-store", $"{_playerName}-state",
                ConsistencyMode.Strong, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (playerState is null)
            {
                return new PlayerState();
            }

            return playerState;
        }

    }
}