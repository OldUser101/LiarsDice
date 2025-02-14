namespace LiarsDice
{
    class Program
    {
        public static ConsoleColor defaultConsoleColor;
        public static Bid? currentBid;
        public static List<Player>? players;
        public static string[] computerNames = ["Max", "Alex", "Sophie", "Charlie", "Jordan", "Taylor", "Morgan", "Casey", "Sam", "Riley", "Tom", "Jake"];      // AI GENERATED
        public static string[] bidFaceNames = ["one", "two", "three", "four", "five", "sixe"];
        public static int currentPlayer;
        public static int N_PLAYERS = 0;
        public static int totalDice = 0;
        public static bool isGameOver = false;
        public static int pSelTtlDispInt = 0;

        public static int UserSelectNumber(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = Console.ReadLine() ?? String.Empty;

                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine($"Invalid value! Must be between {min} and {max} inclusive.");
                    continue;
                }

                if (!int.TryParse(value, out int number))
                {
                    Console.WriteLine($"Invalid value! Must be between {min} and {max} inclusive.");
                    continue;
                }

                if (number < min || number > max)
                {
                    Console.WriteLine($"Invalid value! Must be between {min} and {max} inclusive.");
                    continue;
                }

                return number;
            }
        }

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
            Console.WriteLine($"    {bid.quantity} {bidFaceNames[bid.value - 1]}{(bid.quantity > 1 ? 's' : ' ')}");
        }

        public static void ShowBidChallengeDisplay(Player player, bool isInitial = false)
        {
            Console.Clear();
            ShowTitle();
            Console.WriteLine($"\n{player.GetName()}:");
            Console.Write("    ");
            player.ShowHand();
            Console.WriteLine();

            if (currentBid is null)
            {
                Console.WriteLine($"{player.GetName().ToUpper()} makes the initial bid.");
            }
            else
            {
                ShowBid(currentBid);
            }

            if (!isInitial)
            {
                Console.WriteLine("\n Select Option:");
            }
        }

        public static void ShowPlayerSelectTitle()
        {
            ShowTitle();
            Console.WriteLine();
            Console.WriteLine($"Player {pSelTtlDispInt + 1}");
        }

        public static void ChoosePlayers()
        {
            if (players is null)
                return;

            while (true)
            {
                Console.Write("\nHow many players? (2-12): ");
                string input = Console.ReadLine() ?? String.Empty;

                if (!int.TryParse(input, out N_PLAYERS))
                {
                    Console.WriteLine("Invalid number of players!");
                    continue;
                }

                if (N_PLAYERS > 12 || N_PLAYERS < 2)
                {
                    Console.WriteLine("Invalid number of players!");
                    continue;
                }

                totalDice = N_PLAYERS * 6;
                break;
            }

            List<string> chosenNames = [];
            Random r = new();
            pSelTtlDispInt = 0;

            for (int i = 0; i < N_PLAYERS; i++)
            {
                Menu m = new(["Human", "Computer"], ShowPlayerSelectTitle, null);
                int op = m.ShowMenu();

                switch (op)
                {
                    case 0:
                        while (true)
                        {
                            Console.Write($"\nEnter name for player {pSelTtlDispInt + 1}: ");
                            string name = Console.ReadLine() ?? String.Empty;

                            if (chosenNames.Contains(name))
                            {
                                Console.WriteLine("That name is already taken.");
                                continue;
                            }

                            chosenNames.Add(name);
                            HumanPlayer hp = new(name);
                            players.Add(hp);
                            pSelTtlDispInt++;
                            break;
                        }
                        break;
                    case 1:
                        while (true)
                        {
                            int iName = r.Next() % computerNames.Length;
                            if (chosenNames.Contains(computerNames[iName]))
                            {
                                continue;
                            }
                            chosenNames.Add(computerNames[iName]);
                            players.Add(new ComputerPlayer(computerNames[iName], Math.Clamp(r.NextDouble(), 0, 1)));
                            pSelTtlDispInt++;
                            break;
                        }
                        break;
                }
            }

            Console.Clear();

            ShowTitle();

            Console.WriteLine("\nOkay, here's who's playing:\n");

            foreach (Player p in players)
            {
                Console.WriteLine($"    {p.GetName()}");
            }
        }

        static void ChooseInitialPlayer()
        {
            if (players is null)
                return;

            Random r = new();
            currentPlayer = r.Next() % N_PLAYERS;

            Console.WriteLine($"\n{players[currentPlayer].GetName().ToUpper()} will make the first bid.");
        }

        static void PrepPlayers()
        {
            if (players is null)
                return;

            List<Player> newPlayers = [];
            int ON_PLAYERS = N_PLAYERS;
            for (int i = 0; i < ON_PLAYERS; i++)
            {
                if (players[i].IsOut())
                {
                    Console.WriteLine($"\n{players[i].GetName().ToUpper()} is out.");
                    N_PLAYERS--;
                }
                else
                {
                    players[i].RollDice();
                    newPlayers.Add(players[i]);
                }
            }

            players.Clear();
            foreach (Player p in newPlayers)
            {
                players.Add(p);
            }
        }

        public static bool ValidateBid(Bid bid)
        {
            if (players is null)
                throw new Exception();

            int total = 0;
            foreach (Player p in players)
            {
                foreach (int i in p.GetResults())
                {
                    if (i == bid.value)
                        total++;
                }
            }

            return total >= bid.quantity;
        }

        public static bool MakeChallenge(Bid currentBid)
        {
            if (players is null)
                throw new Exception();

            Console.Clear();
            ShowTitle();

            int lastPlayer = currentPlayer - 1;

            if (lastPlayer < 0)
            {
                lastPlayer = N_PLAYERS - 1;
            }

            Console.WriteLine($"\n{players[currentPlayer].GetName().ToUpper()} challenges {players[lastPlayer].GetName().ToUpper()} for");
            Console.WriteLine($"  {currentBid.quantity} {bidFaceNames[currentBid.value - 1]}{(currentBid.quantity > 1 ? 's' : ' ')}");

            Console.WriteLine("\nPlayer Hands:\n");

            foreach (Player p in players)
            {
                Console.Write($"  {p.GetName().ToUpper()}: ");
                p.ShowHand();
            }

            Console.WriteLine();

            bool valid = ValidateBid(currentBid);

            Console.WriteLine($"The bid was {(valid ? "VALID" : "INVALID")}!");
            Console.Write("Press ANY KEY to continue...");
            Console.ReadKey();
            return valid;
        }

#pragma warning disable IDE0060
        static void Main(string[] args)
#pragma warning restore IDE0060
        {
            Console.CursorVisible = true;
            defaultConsoleColor = Console.ForegroundColor;

            players = [];

            if (players is null)
            {
                return;
            }

            ShowTitle();

            ChoosePlayers();

            Console.Write("\nPress ANY KEY to begin...");
            Console.ReadKey();

            while (!isGameOver)
            {
                Console.Clear();
                ShowTitle();

                currentBid = null;

                PrepPlayers();

                if (isGameOver)
                    break;

                if (players.Count == 1 || N_PLAYERS == 1)
                {
                    Console.WriteLine($"\n{players[0].GetName().ToUpper()} wins the game.\n");
                    Console.Write("Press ANY KEY to quit...");
                    Console.ReadKey();
                    break;
                }

                ChooseInitialPlayer();

                Console.Write("\nPress ANY KEY to begin the round...");
                Console.ReadKey();

                if (players[currentPlayer] as HumanPlayer is not null)
                {
                    ShowBidChallengeDisplay(players[currentPlayer] as HumanPlayer ?? new HumanPlayer("NULL"), true);
                }

                currentBid = players[currentPlayer].MakeBid(new Bid(0, 0), totalDice, true);

                currentPlayer++;

                bool roundInPlay = true;

                while (roundInPlay)
                {
                    currentPlayer %= N_PLAYERS;

                    short move = players[currentPlayer].BidOrChallenge(currentBid, totalDice);

                    switch (move)
                    {
                        case 0:             // BID
                            currentBid = players[currentPlayer].MakeBid(currentBid, totalDice, false);
                            break;
                        case 1:             // CHALLENGE
                            bool challengeSuccess = MakeChallenge(currentBid);
                            if (!challengeSuccess)
                            {
                                int lastPlayer = currentPlayer - 1;

                                if (lastPlayer < 0)
                                {
                                    lastPlayer = N_PLAYERS - 1;
                                }

                                players[lastPlayer].LoseDice();
                            }
                            else
                            {
                                players[currentPlayer].LoseDice();
                            }
                            roundInPlay = false;
                            break;
                        case 2:             // PASS
                            break;
                        case 3:             // EXIT
                            roundInPlay = false;
                            isGameOver = true;
                            break;
                    }

                    currentPlayer++;
                }
            }
        }
    }
}