using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleAppBlackJack
{
    public class Game
    {
        private static string dataPath = "C:/Projekt/XML/ConsoleAppBlackJack/players.xml";

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

        public static void SavePlayer(Player player)
        {
            List<Player> playerList = LoadPlayerList();
            playerList.Add(player);
            string xmlData = XMLConvert.ObjectToXml(playerList);
            File.WriteAllText(dataPath, xmlData);
        }

        private static List<Player> LoadPlayerList()
        {
            string xmlData = File.ReadAllText(dataPath);
            List<Player> playerList = XMLConvert.XMLToObject(xmlData);
            return playerList;
        }

        private bool MakeDeposit(int depositSum)
        {
            bool accepted = false;

            //if (IsActive && depositSum > 0)
            //{
            //    Bankroll += depositSum;
            //    accepted = true;
            //}

            return accepted;
        }

        internal static Player GetPlayerByName(string inputName)
        {
            List<Player> playerList = LoadPlayerList();
            return playerList
                .FirstOrDefault(p => p.Name == inputName);
        }

        public static Stack<Card> ShuffleDeck()
        {
            Random random = new Random();
            List<Card> cardList = new List<Card>();
            Stack<Card> deck = new Stack<Card>();
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

            return deck;
        }
    }
}
