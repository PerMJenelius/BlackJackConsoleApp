using System;
using System.Collections.Generic;

namespace ConsoleAppBlackJack
{
    public class Hand
    {
        public DateTime TimeStamp { get; set; }
        public List<Card> DealerHand { get; set; }
        public List<Card> PlayerHand { get; set; }
        public int DealerHandValue { get; set; }
        public int PlayerHandValue { get; set; }
        public int DealerHandSoftValue { get; set; }
        public int PlayerHandSoftValue { get; set; }
        public double Bet { get; set; }
        public double TransactionAmount { get; set; }
        public bool Split { get; set; }
        public bool Double { get; set; }
        public bool Insurance { get; set; }

        public Hand()
        {
            TimeStamp = DateTime.Now;
            DealerHand = new List<Card>();
            PlayerHand = new List<Card>();
            DealerHandValue = 0;
            PlayerHandValue = 0;
            DealerHandSoftValue = 0;
            PlayerHandSoftValue = 0;
            Bet = 0;
            TransactionAmount = 0;
            Split = false;
            Double = false;
            Insurance = false;
        }
    }
}
