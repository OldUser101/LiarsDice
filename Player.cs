using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string GetName() 
        {
            return this.playerName;
        }

        public abstract Bid MakeBid(Bid currentBid, int totalDice, bool initialBid);
        public abstract bool MakeChallenge(Bid currentBid, int totalDice);
        public abstract short BidOrChallenge(Bid currentBid, int totalDice);
    }

    public class HumanPlayer : Player 
    {
        public HumanPlayer(string name) : base(name, 6) { }

        private string[] playerOptions = { "Bid", "Challenge", "Exit" };

        public void ShowHand()
        {
            foreach (int i in this.dh.GetResults()) 
            {
                Console.Write($"{i} ");
            }
            Console.WriteLine();
        }

        private void ShowBidChallengeDisplay()
        {
            Program.ShowBidChallengeDisplay(this);
        }

        public override Bid MakeBid(Bid currentBid, int totalDice, bool initialBid) 
        {
            return new Bid(0, 0);
        }

        public override bool MakeChallenge(Bid currentBid, int totalDice) 
        {
            return false;
        }

        public override short BidOrChallenge(Bid currentBid, int totalDice) 
        {
            Menu menu = new Menu(playerOptions, ShowBidChallengeDisplay, null);
            short result = (short)menu.ShowMenu();
            return result;
        }
    }

    public class ComputerPlayer : Player
    {
        public ComputerPlayer(string name) : base(name, 6) { }

        public override Bid MakeBid(Bid currentBid, int totalDice, bool initialBid)
        {
            return new Bid(0, 0);
        }

        public override bool MakeChallenge(Bid currentBid, int totalDice)
        {
            return false;
        }

        public override short BidOrChallenge(Bid currentBid, int totalDice)
        {
            return 0;
        }
    }
}
