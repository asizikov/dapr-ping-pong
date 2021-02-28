namespace CloudEng.PingPong.GameManager
{
    public class GameState
    {
        public State Current { get; set; }
        public int PlayerA { get; set; } 
        public int PlayerB { get; set; }
    }

    public enum State
    {
        NewGame = 0,
    }
}