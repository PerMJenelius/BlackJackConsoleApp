using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleAppBlackJack
{
    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Bankroll { get; set; }
        public bool IsActive { get; set; }
        public List<Hand> Hands { get; set; }

        public Player(int id, string name)
        {
            ID = id;
            Name = name;
            Bankroll = 10000;
            IsActive = true;
            Hands = new List<Hand>();
        }

        private Player()
        {

        }

        private static string dataPath = "C:/Projekt/XML/ConsoleAppBlackJack/players.xml";

        public static List<Player> LoadPlayerList()
        {
            string xmlData = File.ReadAllText(dataPath);
            List<Player> playerList = XMLConvert.XMLToObject(xmlData);
            return playerList;
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

        public static Player GetPlayerByName(string inputName)
        {
            List<Player> playerList = LoadPlayerList();
            return playerList
                .FirstOrDefault(p => p.Name == inputName);
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
