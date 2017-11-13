using System;
using System.Collections.Generic;

namespace ConsoleAppBlackJack
{
    class Program
    {
        static List<Player> players = new List<Player>();
        static List<Hand> hands = new List<Hand>();
        static bool active = false;

        static void Main(string[] args)
        {
            Print.Title();
            Print.LoginMenu();
            GetLoginChoice();
            players.Add(Tyler.GetTyler());

            do
            {
                AskForNewRound();
                AskForBet();
                hands[0] = Game.DealStartingHand(hands[0]);
                Tyler.AskForBet(hands[0]);
                AskForAction();
                Tyler.AskForAction(hands);
            } while (players[0].Bankroll >= 5);

            Print.Quit();
        }

        private static void RegisterPlayer()
        {
            Console.WriteLine();
            Console.Write("Please write your name: ");
            string inputName = GetInput();

            while (Player.PlayerNameExists(inputName))
            {
                Console.WriteLine();
                Console.Write("Sorry, that name is taken. Try again: ");
                inputName = GetInput();
            }

            players.Add(new Player(Player.GeneratePlayerId(), inputName));
            Player.SavePlayer(players[0]);
        }

        private static void Login()
        {
            Console.WriteLine();
            Console.Write("Please write your name: ");
            string inputName = GetInput();

            while (!Player.PlayerNameExists(inputName))
            {
                Console.WriteLine();
                Console.Write("Sorry, no player by that name was found. Try again: ");
                inputName = GetInput();
            }
            players.Add(Player.GetPlayerByName(inputName));
            Player.SavePlayer(players[0]);
        }

        private static void EndGame()
        {
            for (int i = 0; i < hands.Count; i++)
            {
                double result = Game.CompareHands(hands[i]);

                hands[i].TransactionAmount = result == 1 ? hands[i].Bet + hands[i].Insurance : hands[i].Bet;
                players[0].Bankroll += hands[i].TransactionAmount;

                players[0].Hands.Add(hands[i]);
                Player.SaveData(players[0]);

                Print.Info(hands, players[0]);
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
            int bet = 0;
            hands.Clear();
            Hand hand = new Hand();

            Print.Info(hands, players[0]);
            Print.BettingChoices(players[0]);

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
            players[0].Bankroll -= bet;
            hands.Add(hand);
            Player.SaveData(players[0]);
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
                        Print.Info(hands, players[0]);

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
                                case "d": hands[i] = Game.Double(hands[i]); players[0].Bankroll -= hands[i].Bet; break;
                                case "p": Game.Split(hands, players[0]); players[0].Bankroll -= hands[1].Bet; break;
                                case "i": hands[i] = Game.Insurance(hands[i]); players[0].Bankroll -= hands[i].Insurance; break;
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
                    hands = Game.DealerRound(hands, players[0]);
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