using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiarsDice
{
    public class Dice(int sides = 6)
    {
        private int _sides = sides;
        private Random _random = new Random();

        public int Roll() 
        {
            return (this._random.Next() % this._sides) + 1;
        }
    }

    public class DiceHand
    {
        private List<Dice> _dice;
        private List<int> _rolledDice;
        private int _nDice;

        public int Length => this._nDice;

        public DiceHand(int nDice) 
        {
            this._dice = new List<Dice>();
            this._rolledDice = new List<int>();
            this._nDice = nDice;
            this.PopulateDice();
        }

        private void PopulateDice() 
        {
            for (int i = 0; i < this._nDice; i++) 
            {
                this._dice.Add(new Dice());
            }
        }

        public void LoseDice() 
        {
            this._dice.RemoveAt(0);
            this._nDice--;
        }

        public void RollAll()
        {
            List<int> results = new List<int>();
            foreach (Dice d in this._dice) 
            {
                results.Add(d.Roll());
            }
            this._rolledDice = results;
        }

        public List<int> GetResults() 
        {
            return this._rolledDice;
        }
    }
}
