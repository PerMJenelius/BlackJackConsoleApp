using System;
using System.Collections.Generic;

namespace ConsoleAppBlackJack
{
    public class Game
    {
        static Stack<Card> deck = new Stack<Card>();

        public static void ShuffleDeck()
        {
            Random random = new Random();
            List<Card> cardList = new List<Card>();
            Rank rank = Rank.Ace;
            Suit suit = Suit.Clubs;

            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0: suit = Suit.Clubs; break;
                    case 1: suit = Suit.Diamonds; break;
                    case 2: suit = Suit.Hearts; break;
                    case 3: suit = Suit.Spades; break;
                }

                for (int j = 0; j < 13; j++)
                {
                    switch (j)
                    {
                        case 0: rank = Rank.Ace; break;
                        case 1: rank = Rank.Two; break;
                        case 2: rank = Rank.Three; break;
                        case 3: rank = Rank.Four; break;
                        case 4: rank = Rank.Five; break;
                        case 5: rank = Rank.Six; break;
                        case 6: rank = Rank.Seven; break;
                        case 7: rank = Rank.Eight; break;
                        case 8: rank = Rank.Nine; break;
                        case 9: rank = Rank.Ten; break;
                        case 10: rank = Rank.Jack; break;
                        case 11: rank = Rank.Queen; break;
                        case 12: rank = Rank.King; break;
                    }
                    cardList.Add(new Card(rank, suit));
                }
            }

            do
            {
                Card card = cardList[random.Next(0, cardList.Count)];
                cardList.Remove(card);
                deck.Push(card);
            } while (cardList.Count > 0);
        }

        internal static Hand DealStartingHand(Hand hand)
        {
            ShuffleDeck();
            DealCard(hand.DealerHand, 1);
            DealCard(hand.PlayerHand, 2);
            return hand;
        }

        public static List<Card> DealCard(List<Card> inputHand, int numberOfCards)
        {
            for (int i = 0; i < numberOfCards; i++)
            {
                inputHand.Add(deck.Pop());
            }

            return inputHand;
        }

        public static List<Hand> EvaluateHands(List<Hand> hands)
        {
            for (int i = 0; i < hands.Count; i++)
            {
                hands[i].PlayerHandValue = CalculateHandValue(hands[i].PlayerHand);
                hands[i].DealerHandValue = CalculateHandValue(hands[i].DealerHand);
                hands[i].PlayerHandSoftValue = hands[i].PlayerHandValue + CountAces(hands[i].PlayerHand);
                hands[i].DealerHandSoftValue = hands[i].DealerHandValue + CountAces(hands[i].DealerHand);
            }

            return hands;
        }

        public static int CalculateHandValue(List<Card> inputHand)
        {
            int sum = 0;

            foreach (var card in inputHand)
            {
                sum += card.Value;
            }

            return sum;
        }

        public static int CountAces(List<Card> inputHand)
        {
            int sum = 0;

            foreach (var card in inputHand)
            {
                if (card.Rank == Rank.Ace)
                {
                    sum = 10;
                }
            }

            return sum;
        }

        public static bool MayInsure(Hand inputHand)
        {
            return inputHand.PlayerHand.Count == 2 && inputHand.DealerHand.Count == 1 && inputHand.DealerHand[0].Rank == Rank.Ace && inputHand.Insurance == 0 && inputHand.Split == false;
        }

        public static Hand Insurance(Hand inputHand)
        {
            if (MayInsure(inputHand))
            {
                inputHand.Insurance = (0.5 * inputHand.Bet);
            }

            return inputHand;
        }

        public static bool MaySplit(Hand inputHand)
        {
            return !inputHand.Split && inputHand.PlayerHand.Count == 2 && inputHand.PlayerHand[0].Value == inputHand.PlayerHand[1].Value;
        }

        public static List<Hand> Split(List<Hand> hands, Player player)
        {
            if (MaySplit(hands[0]))
            {
                Hand hand = new Hand();
                hand.PlayerHand.Add(hands[0].PlayerHand[1]);
                hands[0].PlayerHand.Remove(hands[0].PlayerHand[1]);
                hand.Bet = hands[0].Bet;

                hands.Add(hand);

                hands[0].Split = true;
                hands[1].Split = true;

                hands[0].PlayerHand = DealCard(hands[0].PlayerHand, 1);
                hands[1].PlayerHand = DealCard(hands[1].PlayerHand, 1);

                for (int i = 0; i < hands.Count; i++)
                {
                    if (hands[i].PlayerHand[0].Rank == Rank.Ace)
                    {
                        hands[i].Stand = true;
                    }
                }
            }

            return hands;
        }

        public static Hand Double(Hand inputHand)
        {
            if (inputHand.PlayerHand.Count == 2)
            {
                inputHand.Bet += inputHand.Bet;
                inputHand.PlayerHand = DealCard(inputHand.PlayerHand, 1);
                inputHand.Stand = true;
            }
            return inputHand;
        }

        public static bool CheckForLose(List<Hand> hands)
        {
            int lose = 0;

            for (int i = 0; i < hands.Count; i++)
            {
                lose = hands[i].PlayerHandValue > 21 ? lose + 1 : lose;
            }

            return lose == hands.Count;
        }

        public static bool CheckForStand(List<Hand> hands)
        {
            int stand = 0;

            for (int i = 0; i < hands.Count; i++)
            {
                stand = hands[i].Stand == true ? stand + 1 : stand;
            }

            return stand == hands.Count;
        }

        public static List<Hand> DealerRound(List<Hand> hands, Player player)
        {
            do
            {
                var newHand = DealCard(hands[0].DealerHand, 1);

                for (int i = 0; i < hands.Count; i++)
                {
                    hands[i].DealerHand = newHand;
                }

                hands = EvaluateHands(hands);
                Print.Info(hands, player);
                Print.ContinuePrompt();

            } while (DealOneMore(hands));

            return hands;
        }

        internal static bool DealOneMore(List<Hand> hands)
        {
            return (hands[0].DealerHandSoftValue > hands[0].DealerHandValue && hands[0].DealerHandSoftValue <= 17) ||
                (hands[0].DealerHandSoftValue > 21 && hands[0].DealerHandValue < 17) ||
                (hands[0].DealerHandSoftValue == hands[0].DealerHandValue && hands[0].DealerHandValue < 17);
        }

        internal static double CompareHands(Hand inputHand)
        {
            double output = 0;

            int playerHand = inputHand.PlayerHandSoftValue <= 21 ? inputHand.PlayerHandSoftValue : inputHand.PlayerHandValue;
            int dealerHand = inputHand.DealerHandSoftValue <= 21 ? inputHand.DealerHandSoftValue : inputHand.DealerHandValue;

            bool playerBlackjack = inputHand.PlayerHand.Count == 2 && playerHand == 21 && !inputHand.Split;
            bool dealerBlackjack = inputHand.DealerHand.Count == 2 && dealerHand == 21;

            if (playerHand > 21)
            {
                output = 0;
            }
            else if (dealerHand > 21)
            {
                output = 2;
            }
            else if (dealerBlackjack && !playerBlackjack && inputHand.Insurance == 0)
            {
                output = 0;
            }
            else if (dealerBlackjack && !playerBlackjack && inputHand.Insurance > 0)
            {
                output = 1;
            }
            else if (playerBlackjack && !dealerBlackjack && inputHand.Insurance == 0)
            {
                output = 2.5;
            }
            else if (playerBlackjack && inputHand.Insurance > 0)
            {
                output = 1;
            }
            else if (playerHand < dealerHand)
            {
                output = 0;
            }
            else if (playerHand > dealerHand)
            {
                output = 2;
            }
            else if (playerHand == dealerHand)
            {
                output = 1;
            }

            return output;
        }
    }
}