using System;

namespace Blackjack
{
    class Card{
        private String[] SUITS = {"♠", "♥", "♦", "♣"};
        private String[] NUMBS = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};
        private int suit;
        private int number;

        private bool hidden;

        public Card(int suit, int number, bool hidden){
            this.suit = suit;
            this.number = number;
            this.hidden = hidden;
        }

        public Card(int suit, int number) : this(suit, number, false)
        {}

        // GETTERS
        public int Value {
            get {
                if (number == 0)
                {
                    return 11;
                }
                if (number > 9)
                {
                    return 10;
                }
                return number + 1;
            }
        }

        public bool Hidden { get => this.hidden; }

        // FUNCTIONS
        public void Hide(){
            hidden = true;
        }

        public void Show(){
            hidden = false;
        }

        // DISPLAY
        public override String ToString(){
            return String.Format("{0} {1}", NUMBS[number], SUITS[suit]);
        }
    }
}
