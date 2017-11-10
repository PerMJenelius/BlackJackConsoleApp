using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleAppBlackJack
{
    class Program
    {
        static Player player;
        static List<Hand> hands = new List<Hand>();
        static bool active = false;

        static void Main(string[] args)
        {
            Print.Info(hands, player);
            Print.LoginMenu();
            GetLoginChoice();
            do
            {
                AskForNewRound();
                AskForBet();
                hands[0] = Game.DealStartingHand(hands[0]);
                AskForAction();
            } while (player.Bankroll > 0);
        }

        private static void RegisterPlayer()
        {
            player = AskForPlayerName();

            while (player.IsActive)
            {
                Console.WriteLine();
                Console.WriteLine("Sorry, that name is taken.");
                player = AskForPlayerName();
            }

            active = true;
            player.IsActive = true;
            Game.SavePlayer(player);
        }

        private static void Login()
        {
            player = AskForPlayerName();

            while (!player.IsActive)
            {
                Console.WriteLine();
                Console.WriteLine("Sorry, no player by that name was found.");
                player = AskForPlayerName();
            }

            active = true;
        }

        private static Hand Double(Hand inputHand)
        {
            if (inputHand.PlayerHand.Count == 2)
            {
                player.Bankroll -= inputHand.Bet;
                inputHand.Bet += inputHand.Bet;
                inputHand.Stand = true;
                inputHand.PlayerHand = Game.DealCard(inputHand.PlayerHand, 1);

                hands = Game.EvaluateHands(hands);
                Print.Info(hands, player);

                Console.Write("Press any key to continue");
                Console.ReadKey();
            }
            return inputHand;
        }

        private static void DealerRound()
        {
            do
            {
                var newHand = Game.DealCard(hands[0].DealerHand, 1);

                for (int i = 0; i < hands.Count; i++)
                {
                    hands[i].DealerHand = newHand;
                }

                hands = Game.EvaluateHands(hands);
                Print.Info(hands, player);

                Console.Write("Press any key to continue");
                Console.ReadKey();

            } while (Game.Deal(hands));
        }

        public static void EndGame()
        {
            for (int i = 0; i < hands.Count; i++)
            {
                double result = Game.CompareHands(hands[i]);
                hands[i] = Game.Payout(hands[i], player, result);

                Print.Info(hands, player);
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                if (hands.Count > 1)
                {
                    Console.Write($"Hand {i + 1}: ");
                }

                switch (result)
                {
                    case 0: Console.WriteLine("You lose."); break;
                    case 2: Console.WriteLine("You win!"); break;
                    case 2.5: Console.WriteLine("Blackjack!"); break;
                    case 1:
                        {
                            if (hands[i].Insurance > 0 && hands[0].DealerHand.Count == 2 && hands[0].DealerHandSoftValue == 21)
                            {
                                Console.WriteLine("Insurance pays out.");
                            }
                            else if (hands[i].Insurance > 0 && hands[i].PlayerHand.Count == 2 && hands[i].PlayerHandSoftValue == 21)
                            {
                                Console.WriteLine("Insurance pays out.");
                            }
                            else
                            {
                                Console.WriteLine("It's a draw!");
                            };
                            break;
                        }
                }
                Console.ForegroundColor = color;

                if (hands.Count > 1)
                {
                    Console.ReadKey();
                }
            }

            active = false;
        }

        private static Player AskForPlayerName()
        {
            Console.WriteLine();
            Console.Write("Please write your name: ");
            string inputName = GetInput();

            return Game.PlayerNameExists(inputName) ? Game.GetPlayerByName(inputName) : new Player(Game.GeneratePlayerId(), inputName);
        }

        private static void AskForNewRound()
        {
            Print.NewGameMenu();

            switch (GetInput().ToLower())
            {
                case "q": Print.Quit(); break;
                default: active = true; break;
            }
        }

        private static void AskForBet()
        {
            hands.Clear();
            Hand hand = new Hand();
            int bet = 0;
            Print.Info(hands, player);
            Print.BettingChoices(player);

            switch (GetInput().ToLower())
            {
                case "a": bet = 5; break;
                case "b": bet = 10; break;
                case "c": bet = 20; break;
                case "d": bet = 40; break;
                case "e": bet = 80; break;
                default: bet = 5; break;
            }

            hand.Bet = bet;
            player.Bankroll -= bet;
            hands.Add(hand);
            Game.SaveData(player);
        }

        private static void AskForAction()
        {
            hands = Game.EvaluateHands(hands);

            do
            {
                int count = hands.Count;

                for (int i = 0; i < count; i++)
                {
                    do
                    {
                        Print.Info(hands, player);

                        if (hands.Count > 1)
                        {
                            Console.WriteLine($"Hand {i + 1}");
                        }

                        Print.ActionMenu(hands[i]);

                        if (hands[i].PlayerHandValue >= 21 || hands[i].PlayerHandSoftValue == 21)
                        {
                            hands[i].Stand = true;
                        }
                        else
                        {
                            switch (GetInput().ToLower())
                            {
                                case "h": hands[i].PlayerHand = Game.DealCard(hands[i].PlayerHand, 1); break;
                                case "s": hands[i].Stand = true; break;
                                case "d": hands[i] = Double(hands[i]); break;
                                case "p": Game.Split(hands, player); break;
                                case "i": hands[i] = Game.Insurance(hands[i], player); break;
                                default: hands[i].Stand = true; break;
                            }
                        }

                        hands = Game.EvaluateHands(hands);

                    } while (!hands[i].Stand && hands.Count == count);
                }

                if (Game.CheckForLose(hands))
                {
                    EndGame();
                }
                if (active && Game.CheckForStand(hands))
                {
                    DealerRound();
                    EndGame();
                }

            } while (active);
        }

        private static void GetLoginChoice()
        {
            string input = GetInput().ToLower();

            switch (input)
            {
                case "r": RegisterPlayer(); break;
                case "l": Login(); break;
                case "s": Print.Rules(); break;
            }
        }

        private static string GetInput()
        {
            string input = string.Empty;
            bool success = false;

            do
            {
                try
                {
                    input = Console.ReadLine();
                    success = true;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Please try again.");
                }
            } while (!success);

            return input;
        }
    }
}