namespace CloudEng.PingPong.Player
{
    public class PlayersLuck : IPlayersLuck
    {
        public bool ShouldMissCurrentTake(int counter)
        {
            return counter > 12;
        }
    }
}