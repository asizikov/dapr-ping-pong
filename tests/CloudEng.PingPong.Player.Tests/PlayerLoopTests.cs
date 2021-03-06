using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CloudEng.PingPong.Player.Controllers;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CloudEng.PingPong.Player.Tests
{
    public class PlayerLoopTests
    {
        private readonly PlayerLoop _playerLoop;
        private readonly Mock<DaprClient> _mockDaprClient = new();
        private readonly Mock<IPlayerStateManager> _mockPlayerStateManager = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<IPlayersLuck> _mockPlayersLuck = new();
        private const string PlayerName = "player-x";
        private readonly CancellationToken _token = CancellationToken.None;

        public PlayerLoopTests()
        {
            _playerLoop = new PlayerLoop(NullLogger<PlayerLoop>.Instance, _mockDaprClient.Object,
                _mockPlayerStateManager.Object, _mockConfiguration.Object, _mockPlayersLuck.Object);
        }

        [Fact]
        public async Task When_Ready_Skip_Iteration()
        {
            _mockDaprClient.Setup(client => client.GetStateAsync<PlayerState>("player-state-store",
                    $"{PlayerName}-state", ConsistencyMode.Strong, It.IsAny<IReadOnlyDictionary<string, string>>(),
                    _token))
                .ReturnsAsync(new PlayerState {Current = State.Ready});
            await _playerLoop.ExecuteIterationAsync(PlayerName, _token);
        }
    }
}