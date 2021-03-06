namespace CloudEng.PingPong.Messaging
{
    public class GameProcessEvent
    {
        public string AddressedTo { get; set; }
        public int Ping { get; set; }
    }
}