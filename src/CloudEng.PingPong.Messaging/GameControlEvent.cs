﻿namespace CloudEng.PingPong.Messaging
{
    public class GameControlEvent
    {
        public GameCommand Command { get; set; }
        public string AddressedTo { get; set; }
    }
}
