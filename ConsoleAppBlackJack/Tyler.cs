using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppBlackJack
{
    class Tyler
    {
        static List<Hand> tylersHands = new List<Hand>();
        static string dataPath = "C:/Projekt/XML/ConsoleAppBlackJack/tyler.xml";

        public static Player GetTyler()
        {
            string xmlData = File.ReadAllText(dataPath);
            List<Player> tylerList = XMLConvert.XMLToObject(xmlData);

            return tylerList[0];
        }

        public static List<Hand> GetTylersHands()
        {
            return tylersHands;
        }

        public static void AskForAction(List<Hand> inputHands)
        {
            tylersHands[0].DealerHand = inputHands[0].DealerHand;
            Game.EvaluateHands(tylersHands);

            for (int i = 0; i < tylersHands.Count; i++)
            {
                int tylerHand = tylersHands[i].PlayerHandSoftValue <= 21 ? tylersHands[i].PlayerHandSoftValue : tylersHands[i].PlayerHandValue;
                bool tylerBlackjack = tylersHands[i].PlayerHand.Count == 2 && tylerHand == 21 && !tylersHands[i].Split;
                bool pair = tylersHands[i].PlayerHand.Count == 2 && tylersHands[i].PlayerHand[0].Value == tylersHands[i].PlayerHand[1].Value;

                if (tylerBlackjack)
                {
                    Stand();
                }
                else if (pair)
                {
                    int dealerCard = tylersHands[i].DealerHand[0].Value;
                    int tylerPair = tylersHands[i].PlayerHand[0].Value;

                    if (dealerCard == 2)
                    {
                        if (tylerPair <= 3)
                        {
                            Split();
                        }
                    }
                }
            }
        }

        private static void Split()
        {
            throw new NotImplementedException();
        }

        private static void Stand()
        {
            throw new NotImplementedException();
        }

        public static void AskForBet(Hand inputHand)
        {
            Hand tylersHand = new Hand();
            tylersHand.PlayerHand = Game.DealCard(tylersHand.PlayerHand, 2);
            tylersHand.DealerHand = inputHand.DealerHand;
            tylersHand.Bet = 5;
            tylersHands.Add(tylersHand);
            Game.EvaluateHands(tylersHands);
        }
    }
}
