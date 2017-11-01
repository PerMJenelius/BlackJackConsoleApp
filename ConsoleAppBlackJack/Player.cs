using System.Collections.Generic;

namespace ConsoleAppBlackJack
{
    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Bankroll { get; set; }
        public bool IsActive { get; set; }
        public List<Hand> Hands { get; set; }

        public Player(int id, string name)
        {
            ID = id;
            Name = name;
            Bankroll = 1000000;
            IsActive = false;
            Hands = new List<Hand>();
        }

        private Player()
        {

        }
    }
}
