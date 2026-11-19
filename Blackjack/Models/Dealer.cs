namespace Blackjack
{
    class Dealer{
        private Hand hand;

        public Dealer(){
            this.hand = new Hand();
        }

        // GETTERS
        public Hand Hand { get => this.hand; }

        // MOVES
        public void Deal(Card card, bool hidden){
            hand.Add(card, hidden);
        }

        public void Deal(Card card){
            Deal(card, false);
        }

        public void Stand(){
            hand.Stand = true;
        }

        // HIDE/SHOW CARD
        public void HideCard(int index){
            hand.HideCard(index);
        }

        public void ShowCard(int index){
            hand.ShowCard(index);
        }

        // DISCARD
        public void Discard(){
            hand.Discard();
        }

        // DISPLAY
        public override string ToString(){
            return hand.ToString();
        }
    }
}
