namespace LiarsDice
{
    public abstract class Player
    {
        protected string playerName;
        protected DiceHand dh;

        public Player(string name, int nDice)
        {
            this.dh = new DiceHand(nDice);
            this.playerName = name;
            this.dh.RollAll();
        }

        public void ShowHand()
        {
            foreach (int i in this.dh.GetResults())
            {
                Console.Write($"{i} ");
            }
            Console.WriteLine();
        }

        public List<int> GetResults()
        {
            return dh.GetResults();
        }

        public string GetName()
        {
            return this.playerName;
        }

        public void LoseDice()
        {
            dh.LoseDice();
        }

        public void RollDice()
        {
            dh.RollAll();
        }

        public bool IsOut()
        {
            if (dh.Length <= 0)
            {
                return true;
            }

            return false;
        }

        public abstract Bid MakeBid(Bid currentBid, int totalDice, bool initialBid);
        public abstract short BidOrChallenge(Bid currentBid, int totalDice);
    }

    public class HumanPlayer(string name) : Player(name, 6)
    {
        private readonly string[] playerOptions = ["Raise", "Call"];

        private void ShowBidChallengeDisplay()
        {
            Program.ShowBidChallengeDisplay(this);
        }

        public override Bid MakeBid(Bid currentBid, int totalDice, bool initialBid)
        {
            while (true)
            {
                int face = Program.UserSelectNumber("\n  Face to bid: ", 1, 6);
                int quantity = Program.UserSelectNumber("  Quantity to bid: ", 1, totalDice);
                int faceChange = face - currentBid.value;
                int quantityChange = quantity - currentBid.quantity;

                // If the face decreases, the bid is guaranteed to be invalid.
                if (!initialBid && faceChange < 0)
                {
                    Console.WriteLine("  New bid must have a greater face or quantity that the previous bid!");
                    continue;
                }

                // If the face stays the same, the quantity must rise.
                if (!initialBid && faceChange == 0 && quantityChange < 1)
                {
                    Console.WriteLine("  New bid must have a greater face or quantity that the previous bid!");
                    continue;
                }

                // Otherwise, if the face rises, the quantity can be anything.

                return new Bid(quantity, face);
            }
        }

        public override short BidOrChallenge(Bid currentBid, int totalDice)
        {
            Menu menu = new(playerOptions, ShowBidChallengeDisplay, null);
            short result = (short)menu.ShowMenu();
            return result;
        }
    }

    public class ComputerPlayer(string name, double risk = 0.5) : Player(name, 6)
    {
        private readonly double _risk = Math.Clamp(risk, 0.0, 1.0);

        private static double NCr(int n, int r)
        {
            if (r > n) return 0;
            if (r == 0 || r == n) return 1;
            double res = 1;
            for (int i = 1; i <= r; i++)
            {
                res *= (n - (r - i)) / (double)i;
            }
            return res;
        }


        private static double DiceProb(int q, int n)
        {
            double totalProb = 0;
            for (int x = q; x <= n; x++)
            {
                totalProb += (NCr(n, x) * Math.Pow(1.0 / 6.0, x) * Math.Pow(5.0 / 6.0, n - x));
            }
            return totalProb;
        }

        private int CountMatchingDice(int value)
        {
            int count = 0;

            foreach (int d in this.dh.GetResults())
            {
                if (d == value)
                    count++;
            }

            return count;
        }

        private static int EstimateDiceCount(int remainingDice)
        {
            double probability = 1.0 / 6;
            return (int)Math.Round(probability * remainingDice);
        }

        private Bid MakeFirstBid(int totalDice)
        {
            int bestValue = 0;
            int bestQuantity = 0;

            for (int i = 1; i < 7; i++)
            {
                if (CountMatchingDice(i) > bestQuantity)
                {
                    bestValue = i;
                    bestQuantity = CountMatchingDice(i);
                }
            }

            return new Bid((int)Math.Clamp(bestQuantity * (this._risk + 1.2), 1, totalDice), bestValue);
        }

        public override Bid MakeBid(Bid currentBid, int totalDice, bool initialBid)
        {
            if (initialBid)
            {
                return MakeFirstBid(totalDice);
            }

            int knownCount = CountMatchingDice(currentBid.value);
            int estimatedTotal = knownCount + EstimateDiceCount(totalDice - this.dh.Length);
            double confidence = Math.Clamp((double)estimatedTotal / currentBid.quantity * (this._risk / 3), 0, 1);

            int newQuantity = currentBid.quantity + (int)Math.Round(1 + confidence);

            int newValue = currentBid.value;
            if (this._risk > 0.6 && confidence > 0.5 && currentBid.value < 6)
            {
                newValue++;
            }

            return new Bid(newQuantity, newValue);
        }

        public override short BidOrChallenge(Bid currentBid, int totalDice)
        {
            int knownCount = CountMatchingDice(currentBid.value);
            double prob = DiceProb(currentBid.quantity - knownCount, totalDice - this.dh.Length);

            double challengeThreshold = 0.25 + (0.3 * (1 - this._risk));
            if (prob < challengeThreshold)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
