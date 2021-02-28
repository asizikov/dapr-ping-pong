using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CloudEng.PingPong.GameManager.Tests
{
    public class GameManagerLoopTests
    {
        private GameManagerLoop _gameManagerLoop;

        public GameManagerLoopTests()
        {
            _gameManagerLoop = new GameManagerLoop(NullLogger<GameManagerLoop>.Instance);
        }

        [Fact]
        public void Test1()
        {
        }
    }
}