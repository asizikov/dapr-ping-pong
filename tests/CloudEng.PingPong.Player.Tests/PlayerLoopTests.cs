using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Player.Configuration;
using CloudEng.PingPong.Player.Controllers;
using Dapr.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CloudEng.PingPong.Player.Tests
{
    public class PlayerLoopTests
    {
        private readonly PlayerLoop _playerLoop;
        private readonly Mock<DaprClient> _mockDaprClient = new();
        private readonly Mock<IPlayerStateManager> _mockPlayerStateManager = new();
        private readonly IOptions<PlayerConfigOptions> _playerConfig;
        private readonly Mock<IPlayersLuck> _mockPlayersLuck = new();
        private readonly PlayerConfigOptions _playerConfigOptions;
        private readonly CancellationToken _token = CancellationToken.None;

        public PlayerLoopTests()
        {
            _playerConfigOptions = new PlayerConfigOptions
            {
                OpponentName = "player-y",
                PlayerName = "player-x"
            };
            _playerConfig = Options.Create(_playerConfigOptions);
            _playerLoop = new PlayerLoop(NullLogger<PlayerLoop>.Instance, _mockDaprClient.Object,
                _mockPlayerStateManager.Object, _playerConfig, _mockPlayersLuck.Object);
        }

        [Fact]
        public async Task When_Ready_Skip_Iteration()
        {
            _mockPlayerStateManager.Setup(manager => manager.GetStateAsync(_token))
                .ReturnsAsync(new PlayerState {Current = State.Ready});
            await _playerLoop.ExecuteIterationAsync(_playerConfigOptions.PlayerName, _token).ConfigureAwait(false);
        }
    }
}