using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppBlackJack
{
    class Program
    {
        static Player player;
        static Hand hand;
        static bool running = false;

        static void Main(string[] args)
        {
            GetPlayerInfo();
            do
            {
                InitializeGame();
                AskForBet();
                DealStartingHand();
                AskForAction();
            } while (running);
        }

        private static void AskForNewGame()
        {
            PrintNewGameMenu();

            switch (GetInput().ToLower())
            {
                case "d": running = true; break;
                default: Quit(); break;
            }
        }

        private static void InitializeGame()
        {
            PrintTitle();
            PrintPlayerInfo();
            AskForNewGame();
        }

        private static void AskForBet()
        {
            hand = new Hand();
            int bet = 0;
            PrintTitle();
            PrintPlayerInfo();
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

            PrintTitle();
            PrintPlayerInfo();
            PrintBetAmount();
        }

        private static void DealStartingHand()
        {
            Game.ShuffleDeck();

            PrintTitle();
            PrintPlayerInfo();
            PrintBetAmount();

            Game.DealCard(hand.DealerHand, 2);
            Game.DealCard(hand.PlayerHand, 2);
        }

        private static void AskForAction()
        {
            int dealerSum = Game.EvaluateHand(hand.DealerHand, true);
            int dealerAces = Game.CountAces(hand.DealerHand);
            PrintHand(hand.DealerHand, dealerSum, dealerAces, true);

            int playerSum = Game.EvaluateHand(hand.PlayerHand, false);
            int playerAces = Game.CountAces(hand.PlayerHand);
            PrintHand(hand.PlayerHand, playerSum, playerAces, false);


            Console.ReadKey();
        }

        private static void RegisterPlayer()
        {
            do
            {
                player = AskForPlayerName();
                if (player.IsActive)
                {
                    Console.WriteLine();
                    Console.WriteLine("Sorry, that name is taken.");
                }

            } while (player.IsActive);

            player.IsActive = true;
            Game.SavePlayer(player);
        }

        private static void Login()
        {
            do
            {
                player = AskForPlayerName();
                if (!player.IsActive)
                {
                    Console.WriteLine();
                    Console.WriteLine("Sorry, no player by that name was found.");
                }
            } while (!player.IsActive);
        }

        private static void Quit()
        {
            Console.Clear();
            Console.WriteLine("Now exiting game. Hope to see you again soon!");
            Environment.Exit(1);
        }

        private static Player AskForPlayerName()
        {
            bool playerExists;

            Console.WriteLine();
            Console.Write("Please write your name: ");
            string inputName = GetInput();

            playerExists = Game.PlayerNameExists(inputName);

            if (playerExists)
            {
                return Game.GetPlayerByName(inputName);
            }
            else
            {
                return new Player(Game.GeneratePlayerId(), inputName);
            }
        }

        private static void GetPlayerInfo()
        {
            PrintTitle();
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

            try
            {
                input = Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred. Code: {ex}");
            }

            return input;
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
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Bet: ${hand.Bet}");
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

        private static void PrintHand(List<Card> inputHand, int handValue, int acesCount, bool isDealer)
        {
            if (isDealer)
            {
                Console.WriteLine("Dealer has:");
            }
            else
            {
                Console.WriteLine("You have:");
            }

            if ((isDealer && inputHand.Count > 2) | !isDealer)
            {
                for (int i = 0; i < inputHand.Count; i++)
                {
                    Console.Write($"{inputHand[i].Rank} of {inputHand[i].Suit}");

                    if (i < (inputHand.Count - 2))
                    {
                        Console.Write(", ");
                    }
                    else if (i < (inputHand.Count - 1))
                    {
                        Console.Write(" and ");
                    }
                }
            }
            else if (isDealer && inputHand.Count == 2)
            {
                Console.Write($"{inputHand[0].Rank} of {inputHand[0].Suit} and one in the hole");
            }

            Console.Write($" ({handValue}");

            if (acesCount == 2)
            {
                Console.Write($" or {handValue + 18} or {handValue + 9}");
            }
            else if (acesCount == 1)
            {
                Console.Write($" or {handValue + 9}");
            }

            Console.WriteLine(")");
            Console.WriteLine();
        }

        private static void PrintRules()
        {
            PrintTitle();
            Console.WriteLine();
            Console.WriteLine("Rules:");
            string rules = File.ReadAllText("C:/Projekt/XML/ConsoleAppBlackJack/rules.txt");
            Console.WriteLine(rules);
            Console.ReadKey();
            Console.Clear();
        }
    }
}
