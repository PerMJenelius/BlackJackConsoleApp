using System;

namespace ConsoleAppBlackJack
{
    public class Card
    {
        public Rank Rank { get; set; }
        public Suit Suit { get; set; }
        public int Value { get; set; }

        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
            Value = GetValue(rank);
        }

        private int GetValue(Rank rank)
        {
            int value = 0;

            switch (rank)
            {
                case Rank.Ace: value = 1; break;
                case Rank.Two: value = 2; break;
                case Rank.Three: value = 3; break;
                case Rank.Four: value = 4; break;
                case Rank.Five: value = 5; break;
                case Rank.Six: value = 6; break;
                case Rank.Seven: value = 7; break;
                case Rank.Eight: value = 8; break;
                case Rank.Nine: value = 9; break;
                case Rank.Ten: case Rank.Jack: case Rank.Queen: case Rank.King: value = 10; break;
            }

            return value;
        }

        private Card()
        {

        }
    }

    public enum Rank
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public enum Suit
    {
        Hearts,
        Spades,
        Clubs,
        Diamonds
    }
}
