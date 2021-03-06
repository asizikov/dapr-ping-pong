using Dapr.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CloudEng.PingPong.GameManager.Tests
{
    public class GameManagerLoopTests
    {
        private GameManagerLoop _gameManagerLoop;
        private Mock<DaprClient> _daprClientMock = new();

        public GameManagerLoopTests()
        {
            
            _gameManagerLoop = new GameManagerLoop(NullLogger<GameManagerLoop>.Instance, _daprClientMock.Object);
        }

        [Fact]
        public void Test1()
        {
        }
    }
}