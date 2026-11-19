using System.Collections.Generic;

namespace Blackjack
{
    class Hand{
        private int bet;

        private List<Card> cards;
        private int points;

        private bool hiddenCardFlag;
        private bool aceCardFlag;

        public enum OutcomeHand : int {
            NONE = 0,
            BUST = 1,
            STAND = 2,
            BLACKJACK = 3
        }
        private OutcomeHand outcome;

        public Hand(){
            this.bet = 0;

            this.cards = new List<Card>();
            this.points = 0;

            this.hiddenCardFlag = false;
            this.aceCardFlag = false;

            this.outcome = OutcomeHand.NONE;
        }

        // GETTERS & SETTERS
        public int Bet { get => this.bet; }

        public Card this[int index] {
            get => this.cards[index];
            set => this.cards[index] = value;
        }

        public List<Card> Cards { get => this.cards; }

        public int Points { get => this.points; }

        public bool Bust {
            get => this.outcome == OutcomeHand.BUST;
            set => this.outcome = OutcomeHand.BUST;
        }

        public bool Stand {
            get => this.outcome == OutcomeHand.STAND;
            set => this.outcome = OutcomeHand.STAND;
        }

        public bool Blackjack {
            get => this.outcome == OutcomeHand.BLACKJACK;
            set => this.outcome = OutcomeHand.BLACKJACK;
        }

        public OutcomeHand Outcome {
            get => this.outcome;
            set => this.outcome = value;
        }

        // BETS
        public void MakeBet(int amount){
            this.bet += amount;
        }

        // DRAW CARD
        public void Add(Card card, bool hidden){
            this.cards.Add(card);
            if ( !hidden ){
                AddState(card);
            }
            else{
                this.hiddenCardFlag = true;
                card.Hide();
            }
        }

        public Card Remove(int index){
            Card card = this.cards[index];

            this.cards.RemoveAt(index);
            this.RemoveState(card);

            return card;
        }

        // HIDE/SHOW CARD
        public void HideCard(int index){
            Card card = this.cards[index];

            card.Hide();
            this.RemoveState(card);
            this.hiddenCardFlag = true;
        }

        public void ShowCard(int index){
            Card card = this.cards[index];

            card.Show();
            this.AddState(card);
            this.hiddenCardFlag = false;
        }

        public void AddState(Card card){
            this.points += card.Value;
            if (card.Value == 11){
                this.aceCardFlag = true;
            }

            if (this.points == 21){
                this.Blackjack = true;
            }
            else if (this.points > 21){
                if (!this.aceCardFlag){
                    this.Bust = true;
                }
                else{
                    this.points -= 10;
                    this.aceCardFlag = false;
                }
            }
        }

        public void RemoveState(Card card){
            this.points -= card.Value;
            if (card.Value == 11){
                this.aceCardFlag = false;
            }
        }

        // DISCARD
        public void Discard(){
            this.bet = 0;

            this.cards.Clear();
            this.points = 0;

            this.hiddenCardFlag = false;
            this.aceCardFlag = false;

            this.outcome = OutcomeHand.NONE;
        }

        // DISPLAY
        public override string ToString(){
            string result = "";
            foreach (Card card in this.cards){
                result += card.ToString() + " ";
            }
            return result;
        }
    }
}
