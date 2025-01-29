using System;

namespace LiarsDice
{
    class Program
    {
        public static ConsoleColor defaultConsoleColor;
        public static Bid currentBid;
        public static List<Player> players;
        public static string[] computerNames = { "Max", "Alex", "Sophie", "Charlie", "Jordan", "Taylor", "Morgan", "Casey", "Sam", "Riley", "Tom" };      // AI GENERATED
        public static string[] bidFaceNames = { "ones", "twos", "threes", "fours", "fives", "sixes" };
        public static int currentPlayer;
        public const int N_PLAYERS = 6;

        public static void ShowTitle() 
        {
            Console.Write("=== ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("LIAR'S DICE");
            Console.ForegroundColor = defaultConsoleColor;
            Console.Write(" ===\n");
            Console.WriteLine("  (c) Nathan Gill");
        }

        public static void ShowBid(Bid bid) 
        {
            Console.WriteLine("Current Bid:");
            Console.WriteLine($"    {bid.quantity} {bidFaceNames[bid.value]}");
        }

        public static void ShowBidChallengeDisplay(HumanPlayer player) 
        {
            ShowTitle();
            Console.WriteLine($"\n{player.GetName()}:");
            Console.Write("    ");
            player.ShowHand();
            Console.WriteLine();
            ShowBid(currentBid);
            Console.WriteLine("\n Select Option:");
        }

        public static void ChoosePlayers() 
        {
            Console.Write("\nEnter your name: ");
            string name = Console.ReadLine() ?? String.Empty;

            players.Add(new HumanPlayer(name));

            List<int> chosenNames = new List<int>();
            Random r = new Random();

            Console.WriteLine($"\n{name}, you are playing with:\n");

            for (int i = 0; i < N_PLAYERS - 1; i++)
            {
                int iName = r.Next() % computerNames.Length;
                if (chosenNames.Contains(iName))
                {
                    i--;
                    continue;
                }
                chosenNames.Add(iName);
                players.Add(new ComputerPlayer(computerNames[iName]));
                Console.WriteLine($"  {computerNames[iName]}");
            }

            currentPlayer = r.Next() % N_PLAYERS;

            if (currentPlayer == 0)
            {
                Console.WriteLine("\nYOU will make the first bid.");
            }
            else
            {
                Console.WriteLine($"\n{players[currentPlayer].GetName().ToUpper()} will make the first bid.");
            }
        }

        static void Main(string[] args)
        {
            defaultConsoleColor = Console.ForegroundColor;

            currentBid = new Bid(1, 3);

            players = new List<Player>();

            ShowTitle();

            ChoosePlayers();

            Console.Write("\nPress ANY KEY to begin...");





            Console.ReadLine();
        }
    }
}