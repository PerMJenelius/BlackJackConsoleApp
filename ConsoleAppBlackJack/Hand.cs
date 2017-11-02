using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppBlackJack
{
    public class Hand
    {
        public DateTime TimeStamp { get; set; }
        public List<Card> DealerHand { get; set; }
        public List<Card> PlayerHand { get; set; }
        public int Bet { get; set; }
        public int TransactionAmount { get; set; }

        public Hand()
        {
            TimeStamp = DateTime.Now;
            DealerHand = new List<Card>();
            PlayerHand = new List<Card>();
            Bet = 0;
            TransactionAmount = 0;
        }
    }
}
