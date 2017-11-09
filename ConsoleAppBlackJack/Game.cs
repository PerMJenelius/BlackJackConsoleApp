using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleAppBlackJack
{
    public class Game
    {
        private static string dataPath = "C:/Projekt/XML/ConsoleAppBlackJack/players.xml";
        static Stack<Card> deck = new Stack<Card>();

        public static int GeneratePlayerId()
        {
            int id = 1;
            List<Player> playerList = LoadPlayerList();
            if (playerList.Count > 0)
            {
                int maxId = playerList
                    .Max(p => p.ID);
                id = ++maxId;
            }
            return id;
        }

        private static List<Player> LoadPlayerList()
        {
            string xmlData = File.ReadAllText(dataPath);
            List<Player> playerList = XMLConvert.XMLToObject(xmlData);
            return playerList;
        }

        internal static Player GetPlayerByName(string inputName)
        {
            List<Player> playerList = LoadPlayerList();
            return playerList
                .FirstOrDefault(p => p.Name == inputName);
        }

        public static bool PlayerNameExists(string input)
        {
            List<Player> playerList = LoadPlayerList();
            bool exists = false;

            foreach (var player in playerList)
            {
                if (player.Name == input)
                {
                    exists = true; break;
                }
            }
            return exists;
        }

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
                deck.Push(card);
                cardList.Remove(card);
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

        public static int CalculateHandValue(List<Card> inputHand)
        {
            int sum = 0;

            foreach (var card in inputHand)
            {
                sum += card.Value;
            }

            return sum;
        }

        internal static double CompareHands(Hand inputHand)
        {
            double output = 0;

            int playerHand = inputHand.PlayerHandSoftValue <= 21 ? inputHand.PlayerHandSoftValue : inputHand.PlayerHandValue;
            int dealerHand = inputHand.DealerHandSoftValue <= 21 ? inputHand.DealerHandSoftValue : inputHand.DealerHandValue;

            bool playerBlackjack = inputHand.PlayerHand.Count == 2 && playerHand == 21 && !inputHand.Split ? true : false;
            bool dealerBlackjack = inputHand.DealerHand.Count == 2 && dealerHand == 21 ? true : false;

            if (playerBlackjack && !dealerBlackjack && inputHand.Insurance == 0)
            {
                output = 2.5;
            }
            else if (playerBlackjack && inputHand.Insurance > 0)
            {
                output = 1;
            }
            else if (dealerBlackjack && !playerBlackjack && inputHand.Insurance > 0)
            {
                output = 1;
            }
            else if (dealerBlackjack && !playerBlackjack && inputHand.Insurance == 0)
            {
                output = 0;
            }
            else if (playerHand > 21)
            {
                output = 0;
            }
            else if (dealerHand > 21)
            {
                output = 2;
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

        public static Hand Insurance(Hand inputHand, Player player)
        {
            if (inputHand.PlayerHand.Count == 2 && inputHand.DealerHand.Count == 1 && inputHand.DealerHand[0].Rank == Rank.Ace && inputHand.Insurance == 0 && inputHand.Split == false)
            {
                inputHand.Insurance = (0.5 * inputHand.Bet);
                player.Bankroll -= inputHand.Insurance;
                SaveData(player);

                if (inputHand.PlayerHand.Count == 2 && inputHand.PlayerHandSoftValue == 21)
                {
                    Program.EndGame();
                }
            }

            return inputHand;
        }

        public static List<Hand> Split(List<Hand> hands, Player player)
        {
            if (hands[0].PlayerHand.Count == 2 && hands[0].PlayerHand[0].Value == hands[0].PlayerHand[1].Value)
            {
                Hand hand2 = new Hand();
                hand2.PlayerHand.Add(hands[0].PlayerHand[1]);
                hands[0].PlayerHand.Remove(hands[0].PlayerHand[1]);
                hand2.Bet = hands[0].Bet;
                player.Bankroll -= hand2.Bet;
                hands[0].Split = true;
                hand2.Split = true;
                hands.Add(hand2);
                player.Hands.Add(hand2);
                Game.SaveData(player);
                hands[0].PlayerHand = Game.DealCard(hands[0].PlayerHand, 1);
                hand2.PlayerHand = Game.DealCard(hand2.PlayerHand, 1);

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

        public static void SavePlayer(Player player)
        {
            List<Player> playerList = LoadPlayerList();
            playerList.Add(player);
            string xmlData = XMLConvert.ObjectToXml(playerList);
            File.WriteAllText(dataPath, xmlData);
        }

        public static void SaveData(Player player)
        {
            List<Player> playerList = LoadPlayerList();
            Player oldPlayer = playerList
                .FirstOrDefault(p => p.ID == player.ID);
            playerList.Remove(oldPlayer);
            playerList.Add(player);
            string xmlData = XMLConvert.ObjectToXml(playerList);
            File.WriteAllText(dataPath, xmlData);
        }
    }
}