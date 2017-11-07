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
        static Hand hand;
        static bool running = false;
        static bool active = true;

        static void Main(string[] args)
        {
            GetPlayerInfo();
            do
            {
                AskForNewRound();
                AskForBet();
                DealStartingHand();
                AskForAction();
            } while (running);
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

        private static void DealStartingHand()
        {
            Game.ShuffleDeck();

            PrintTitle();
            PrintPlayerInfo();
            PrintBetAmount();

            Game.DealCard(hand.DealerHand, 1);
            Game.DealCard(hand.PlayerHand, 2);
        }

        private static void Surrender()
        {
            Console.WriteLine("Surrender");
        }

        private static void EvenMoney()
        {
            Console.WriteLine("Even Money");
        }

        private static void Insurance()
        {
            Console.WriteLine("Insurance");
        }

        private static void Split()
        {
            Console.WriteLine("Split");
        }

        private static void Double()
        {
            player.Bankroll -= hand.Bet;
            hand.Bet += hand.Bet;
            hand.PlayerHand = Game.DealCard(hand.PlayerHand, 1);

            PrintTitle();
            PrintPlayerInfo();
            PrintBetAmount();
            PrintHands(hand);

            DealerTurn();
        }

        private static void DealerTurn()
        {
            while (hand.DealerHandValue < 17 && hand.DealerHandSoftValue < 17)
            {
                hand.DealerHand = Game.DealCard(hand.DealerHand, 1);
                EvaluateHands();

                PrintTitle();
                PrintPlayerInfo();
                PrintBetAmount();
                PrintHands(hand);

                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }

            EndGame();
        }

        private static void EvaluateHands()
        {
            hand = Game.EvaluateHand(hand);

            int playerHand = hand.PlayerHandSoftValue > hand.PlayerHandValue && hand.PlayerHandSoftValue <= 21 ? hand.PlayerHandSoftValue : hand.PlayerHandValue;
            int dealerHand = hand.DealerHandSoftValue > hand.DealerHandValue && hand.DealerHandSoftValue <= 21 ? hand.DealerHandSoftValue : hand.DealerHandValue;

            if (playerHand > 21 || dealerHand > 21)
            {
                EndGame();
            }
        }

        private static void EndGame()
        {
            double result = Game.CompareHands(hand);
            hand.TransactionAmount = result * hand.Bet;
            player.Bankroll += hand.TransactionAmount;
            player.Hands.Add(hand);
            Game.SaveData(player);

            PrintTitle();
            PrintPlayerInfo();
            PrintBetAmount();
            PrintHands(hand);
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            switch (result)
            {
                case 0: Console.WriteLine("You lose."); break;
                case 2: Console.WriteLine("You win!"); break;
                case 2.5: Console.WriteLine("Blackjack!"); break;
                case 1: Console.WriteLine("It's a draw!"); break;
            }
            Console.ForegroundColor = color;

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
            Game.SaveData(player);

            PrintTitle();
            PrintPlayerInfo();
            PrintBetAmount();
        }

        private static void AskForAction()
        {
            EvaluateHands();

            do
            {
                PrintTitle();
                PrintPlayerInfo();
                PrintBetAmount();
                PrintHands(hand);
                PrintActionMenu();

                switch (GetInput().ToLower())
                {
                    case "h": hand.PlayerHand = Game.DealCard(hand.PlayerHand, 1); break;
                    case "s": DealerTurn(); break;
                    case "d": Double(); break;
                    case "p": Split(); break;
                    case "i": Insurance(); break;
                    case "e": EvenMoney(); break;
                    case "u": Surrender(); break;
                }

                EvaluateHands();

            } while (active);
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

        private static void PrintHands(Hand inputHand)
        {
            Console.WriteLine("Dealer has:");

            for (int i = 0; i < inputHand.DealerHand.Count; i++)
            {
                Console.Write($"{inputHand.DealerHand[i].Rank} of {inputHand.DealerHand[i].Suit}");

                if (i < (inputHand.DealerHand.Count - 2))
                {
                    Console.Write(", ");
                }
                else if (i < (inputHand.DealerHand.Count - 1))
                {
                    Console.Write(" and ");
                }
            }

            Console.Write($" ({hand.DealerHandValue}");

            if (hand.DealerHandSoftValue > hand.DealerHandValue && hand.DealerHandSoftValue <= 21)
            {
                Console.Write($" or {hand.DealerHandSoftValue}");
            }

            Console.WriteLine(")");
            Console.WriteLine();
            Console.WriteLine("You have:");

            for (int i = 0; i < inputHand.PlayerHand.Count; i++)
            {
                Console.Write($"{inputHand.PlayerHand[i].Rank} of {inputHand.PlayerHand[i].Suit}");

                if (i < (inputHand.PlayerHand.Count - 2))
                {
                    Console.Write(", ");
                }
                else if (i < (inputHand.PlayerHand.Count - 1))
                {
                    Console.Write(" and ");
                }
            }

            Console.Write($" ({hand.PlayerHandValue}");

            if (hand.PlayerHandSoftValue > hand.PlayerHandValue && hand.PlayerHandSoftValue <= 21)
            {
                Console.Write($" or {hand.PlayerHandSoftValue}");
            }

            Console.WriteLine(")");
            Console.WriteLine();
        }

        private static void PrintActionMenu()
        {
            Console.WriteLine("[H]it");
            Console.WriteLine("[S]tand");
            Console.WriteLine("[D]ouble");

            if (hand.Split)
            {
                Console.WriteLine("S[P]lit");
            }
            if (hand.Insurance)
            {
                Console.WriteLine("[I]nsurance");
            }

            Console.WriteLine("S[U]rrender");
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
