using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppBlackJack
{
    class Print
    {
        public static void LoginMenu()
        {
            Console.WriteLine();
            Console.WriteLine("[R]egister new player");
            Console.WriteLine("[L]ogin");
            Console.WriteLine("[S]how rules");
        }

        public static void Rules()
        {
            Title();
            Console.WriteLine();
            Console.WriteLine("Rules:");
            string rules = File.ReadAllText("C:/Projekt/XML/ConsoleAppBlackJack/rules.txt");
            Console.WriteLine(rules);
            Console.ReadKey();
            Console.Clear();
        }

        public static void Info(List<Hand> hands, Player player)
        {
            Title();

            if (player != null)
            {
                PlayerInfo(player);
            }
            if (hands.Count > 0)
            {
                BetAmount(hands);
                Hands(hands);
            }
        }

        public static void NewGameMenu()
        {
            Console.WriteLine();
            Console.WriteLine("[D]eal a new hand");
            Console.WriteLine("[Q]uit");
        }

        public static void BetAmount(List<Hand> hands)
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

        public static void PlayerInfo(Player player)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Player: {player.Name}  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Bankroll: ${ player.Bankroll}");
            Console.ForegroundColor = color;
        }

        public static void Title()
        {
            Console.Clear();
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("BlackJack Console App v 1.0");
            Console.WriteLine("===========================");
            Console.ForegroundColor = color;
        }

        public static void ActionMenu(Hand inputHand)
        {
            Console.WriteLine("[H]it");
            Console.WriteLine("[S]tand");

            if (inputHand.PlayerHand.Count == 2)
            {
                Console.WriteLine("[D]ouble");
            }
            if (Game.MaySplit(inputHand))
            {
                Console.WriteLine("S[P]lit");
            }
            if (Game.MayInsure(inputHand))
            {
                Console.WriteLine("[I]nsurance");
            }
        }

        public static void Hands(List<Hand> hands)
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

            Console.Write($" ({hands[0].DealerHandValue}");

            if (hands[0].DealerHandSoftValue > hands[0].DealerHandValue && hands[0].DealerHandSoftValue <= 21)
            {
                Console.Write($" or {hands[0].DealerHandSoftValue}");
            }

            Console.WriteLine(")");
            Console.WriteLine();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
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
            Console.ForegroundColor = color;
        }

        public static void BettingChoices(Player player)
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

        public static void Quit()
        {
            Console.Clear();
            Console.WriteLine("Now exiting game. Hope to see you again soon!");
            Environment.Exit(1);
        }

        public static void ContinuePrompt()
        {
            Console.Write("Press any key to continue");
            Console.ReadKey();
        }
    }
}
