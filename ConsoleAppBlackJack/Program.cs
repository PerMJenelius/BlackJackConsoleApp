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

            Game.DealCard(hands[0].DealerHand, 1);
            Game.DealCard(hands[0].PlayerHand, 2);
        }

        private static Hand Insurance(Hand inputHand)
        {
            if (inputHand.PlayerHand.Count == 2 && hands[0].DealerHand.Count == 1 && hands[0].DealerHand[0].Rank == Rank.Ace && !inputHand.Insurance)
            {
                inputHand.Bet += (0.5 * inputHand.Bet);
                inputHand.Insurance = true;

                if (inputHand.PlayerHand.Count == 2 && inputHand.PlayerHandSoftValue == 21)
                {
                    EndGame();
                }
            }

            return inputHand;
        }

        private static void Split()
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
        }

        private static Hand Double(Hand inputHand)
        {
            if (inputHand.PlayerHand.Count == 2)
            {
                player.Bankroll -= inputHand.Bet;
                inputHand.Bet += inputHand.Bet;
                inputHand.Stand = true;
                inputHand.PlayerHand = Game.DealCard(inputHand.PlayerHand, 1);

                EvaluateHands();

                PrintTitle();
                PrintPlayerInfo();
                PrintBetAmount();
                PrintHands();

                Console.Write("Press any key to continue");
                Console.ReadKey();
            }
            return inputHand;
        }

        private static void DealerRound()
        {
            bool deal = true;

            do
            {
                deal = false;

                if (hands[0].DealerHandSoftValue > hands[0].DealerHandValue && hands[0].DealerHandSoftValue <= 17)
                {
                    deal = true;
                }
                else if (hands[0].DealerHandSoftValue > 21 && hands[0].DealerHandValue < 17)
                {
                    deal = true;
                }
                else if (hands[0].DealerHandSoftValue == hands[0].DealerHandValue && hands[0].DealerHandValue < 17)
                {
                    deal = true;
                }

                if (deal)
                {
                    var newHand = Game.DealCard(hands[0].DealerHand, 1);

                    for (int i = 0; i < hands.Count; i++)
                    {
                        hands[i].DealerHand = newHand;
                    }

                    EvaluateHands();
                    PrintTitle();
                    PrintPlayerInfo();
                    PrintBetAmount();
                    PrintHands();

                    Console.Write("Press any key to continue");
                    Console.ReadKey();
                }

            } while (deal);

            EndGame();
        }

        private static void EvaluateHands()
        {
            for (int i = 0; i < hands.Count; i++)
            {
                hands[i] = Game.EvaluateHand(hands[i]);
            }
        }

        private static void EndGame()
        {
            double result = 0;

            for (int i = 0; i < hands.Count; i++)
            {
                result = Game.CompareHands(hands[i]);
                hands[i].TransactionAmount = result * hands[i].Bet;
                player.Bankroll += hands[i].TransactionAmount;
                player.Hands.Add(hands[i]);

                Game.SaveData(player);

                PrintTitle();
                PrintPlayerInfo();
                PrintBetAmount();
                PrintHands();

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
                            if (hands[i].Insurance && hands[0].DealerHand.Count == 2 && hands[0].DealerHandSoftValue == 21)
                            {
                                Console.WriteLine("Insurance pays out.");
                            }
                            else if (hands[i].Insurance && hands[i].PlayerHand.Count == 2 && hands[i].PlayerHandSoftValue == 21)
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

                Console.ReadKey();
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
            hands.Clear();
            Hand hand = new Hand();
            int bet = 5;
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
            hands.Add(hand);
            Game.SaveData(player);
        }

        private static void AskForAction()
        {
            EvaluateHands();

            do
            {
                PrintTitle();
                PrintPlayerInfo();
                PrintBetAmount();
                PrintHands();

                int count = hands.Count;

                for (int i = 0; i < count; i++)
                {
                    if (!hands[i].Stand)
                    {
                        if (hands.Count > 1)
                        {
                            Console.WriteLine($"Hand {i + 1}");
                        }

                        PrintActionMenu(hands[i]);

                        switch (GetInput().ToLower())
                        {
                            case "h": hands[i].PlayerHand = Game.DealCard(hands[i].PlayerHand, 1); break;
                            case "s": hands[i].Stand = true; break;
                            case "d": hands[i] = Double(hands[i]); break;
                            case "p": Split(); break;
                            case "i": hands[i] = Insurance(hands[i]); break;
                            default: hands[i].Stand = true; break;
                        }
                    }
                }

                EvaluateHands();

                if (CheckForStand())
                {
                    DealerRound();
                }
                if (CheckForLose())
                {
                    EndGame();
                }

            } while (active);
        }

        private static bool CheckForLose()
        {
            int lose = 0;

            for (int i = 0; i < hands.Count; i++)
            {
                lose = hands[i].PlayerHandValue > 21 ? lose + 1 : lose;
            }

            return lose == hands.Count;
        }

        private static bool CheckForStand()
        {
            int stand = 0;

            for (int i = 0; i < hands.Count; i++)
            {
                stand = hands[i].Stand == true || hands[i].PlayerHandValue == 21 || hands[i].PlayerHandSoftValue == 21 ? stand + 1 : stand;
            }

            return stand == hands.Count;
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
            Console.WriteLine($"Bet: ${hands[0].Bet}");
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
            if (inputHand.PlayerHand.Count == 2 && inputHand.DealerHand.Count == 1 && inputHand.DealerHand[0].Rank == Rank.Ace && !inputHand.Insurance)
            {
                Console.WriteLine("[I]nsurance");
            }
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