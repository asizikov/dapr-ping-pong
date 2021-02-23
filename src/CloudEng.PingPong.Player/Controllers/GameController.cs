using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CloudEng.PingPong.Player.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }
    }
}