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
        static bool running = false;
        static bool active = true;

        static void Main(string[] args)
        {
            GetPlayerInfo();
            do
            {
                AskForNewRound();
                AskForBet();
                hands[0] = Game.DealStartingHand(hands[0]);
                AskForAction();
            } while (running);
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
                PrintInfo();

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
                PrintInfo();

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

                PrintInfo();
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

        private static void Quit()
        {
            Console.Clear();
            Console.WriteLine("Now exiting game. Hope to see you again soon!");
            Environment.Exit(1);
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
            PrintNewGameMenu();

            switch (GetInput().ToLower())
            {
                case "q": Quit(); break;
                default: running = true; active = true; break;
            }
        }

        private static void AskForBet()
        {
            hands.Clear();
            Hand hand = new Hand();
            int bet = 0;
            PrintInfo();
            PrintBettingChoices();

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
                        PrintInfo();

                        if (hands.Count > 1)
                        {
                            Console.WriteLine($"Hand {i + 1}");
                        }

                        PrintActionMenu(hands[i]);

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

        private static void GetPlayerInfo()
        {
            PrintInfo();
            do
            {
                PrintLoginMenu();
                GetLoginChoice();
            } while (!running);
        }

        private static void GetLoginChoice()
        {
            string input = GetInput().ToLower();

            switch (input)
            {
                case "r": RegisterPlayer(); running = true; break;
                case "l": Login(); running = true; break;
                case "s": PrintRules(); break;
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

        private static void PrintInfo()
        {
            PrintTitle();

            if (player != null)
            {
                PrintPlayerInfo();
            }
            if (hands.Count > 0)
            {
                PrintBetAmount();
                PrintHands();
            }
        }

        private static void PrintTitle()
        {
            Console.Clear();
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("BlackJack Console App v 1.0");
            Console.WriteLine("===========================");
            Console.ForegroundColor = color;
        }

        private static void PrintPlayerInfo()
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Player: {player.Name}  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Bankroll: ${ player.Bankroll}");
            Console.ForegroundColor = color;
        }

        private static void PrintBetAmount()
        {
            double bet = 0;

            for (int i = 0; i < hands.Count; i++)
            {
                bet += hands[i].Bet;
            }

            bet += hands[0].Insurance;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Bet: ${bet}");
            Console.ForegroundColor = color;
            Console.WriteLine();
        }

        private static void PrintLoginMenu()
        {
            Console.WriteLine();
            Console.WriteLine("[R]egister new player");
            Console.WriteLine("[L]ogin");
            Console.WriteLine("[S]how rules");
        }

        private static void PrintNewGameMenu()
        {
            Console.WriteLine();
            Console.WriteLine("[D]eal a new hand");
            Console.WriteLine("[Q]uit");
        }

        private static void PrintBettingChoices()
        {
            char letter = 'A';

            Console.WriteLine();

            if (player.Bankroll >= 5)
            {
                Console.WriteLine("Please choose a bet amount:");

                for (int i = 5; i < 81; i = i * 2)
                {
                    switch (i)
                    {
                        case 5: letter = 'A'; break;
                        case 10: letter = 'B'; break;
                        case 20: letter = 'C'; break;
                        case 40: letter = 'D'; break;
                        case 80: letter = 'E'; break;
                        default:
                            break;
                    }

                    if (player.Bankroll >= i)
                    {
                        Console.WriteLine($"[{letter}] ${i}");
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Sorry, you're out of money.");
                Quit();
            }
        }

        private static void PrintHands()
        {
            Console.WriteLine("Dealer has:");

            for (int i = 0; i < hands[0].DealerHand.Count; i++)
            {
                Console.Write($"{hands[0].DealerHand[i].Rank} of {hands[0].DealerHand[i].Suit}");

                if (i < (hands[0].DealerHand.Count - 2))
                {
                    Console.Write(", ");
                }
                else if (i < (hands[0].DealerHand.Count - 1))
                {
                    Console.Write(" and ");
                }
            }

            Console.Write($" ({Program.hands[0].DealerHandValue}");

            if (Program.hands[0].DealerHandSoftValue > Program.hands[0].DealerHandValue && Program.hands[0].DealerHandSoftValue <= 21)
            {
                Console.Write($" or {Program.hands[0].DealerHandSoftValue}");
            }

            Console.WriteLine(")");
            Console.WriteLine();

            Console.WriteLine("You have:");

            for (int i = 0; i < hands.Count; i++)
            {
                if (hands.Count > 1)
                {
                    Console.Write($"({i + 1}) ");
                }

                for (int j = 0; j < hands[i].PlayerHand.Count; j++)
                {
                    Console.Write($"{hands[i].PlayerHand[j].Rank} of {hands[i].PlayerHand[j].Suit}");

                    if (j < (hands[i].PlayerHand.Count - 2))
                    {
                        Console.Write(", ");
                    }
                    else if (j < (hands[i].PlayerHand.Count - 1))
                    {
                        Console.Write(" and ");
                    }
                }

                Console.Write($" ({hands[i].PlayerHandValue}");

                if (hands[i].PlayerHandSoftValue > hands[i].PlayerHandValue && hands[i].PlayerHandSoftValue <= 21)
                {
                    Console.Write($" or {hands[i].PlayerHandSoftValue}");
                }

                Console.WriteLine(")");
            }
            Console.WriteLine();
        }

        private static void PrintActionMenu(Hand inputHand)
        {
            Console.WriteLine("[H]it");
            Console.WriteLine("[S]tand");

            if (inputHand.PlayerHand.Count == 2)
            {
                Console.WriteLine("[D]ouble");
            }
            if (!inputHand.Split && inputHand.PlayerHand.Count == 2 && inputHand.PlayerHand[0].Value == inputHand.PlayerHand[1].Value)
            {
                Console.WriteLine("S[P]lit");
            }
            if (inputHand.PlayerHand.Count == 2 && inputHand.DealerHand.Count == 1 && inputHand.DealerHand[0].Rank == Rank.Ace && inputHand.Insurance == 0 && inputHand.Split == false)
            {
                Console.WriteLine("[I]nsurance");
            }
        }

        private static void PrintRules()
        {
            PrintInfo();
            Console.WriteLine();
            Console.WriteLine("Rules:");
            string rules = File.ReadAllText("C:/Projekt/XML/ConsoleAppBlackJack/rules.txt");
            Console.WriteLine(rules);
            Console.ReadKey();
            Console.Clear();
        }
    }
}