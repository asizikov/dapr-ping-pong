using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Configuration;

namespace CloudEng.PingPong.Player.Controllers
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
                cancellationToken: cancellationToken);
        }

        public async Task<PlayerState> GetStateAsync(CancellationToken cancellationToken)
        {
            var playerState = await _daprClient.GetStateAsync<PlayerState>("player-state-store", $"{_playerName}-state",
                ConsistencyMode.Strong, cancellationToken: cancellationToken);
            if (playerState is null)
            {
                return new PlayerState();
            }

            return playerState;
        }

    }
}