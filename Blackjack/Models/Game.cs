using System.Collections.Generic;

namespace Blackjack
{
    class Game{
        private Deck deck;
        private Deck discard;
        private Dealer dealer;
        private List<Player> players;

        public Game(List<Player> players, int nrDecks){
            this.deck = new Deck(nrDecks);
            this.discard = new Deck(0);

            this.dealer = new Dealer();
            this.players = players;
        }

        public Game(List<Player> players): this(players, 1)
        {}

        // GETTERS
        public Deck Deck { get => deck; }

        public Dealer Dealer { get => dealer; }

        public Player this[int index] {
            get => this.players[index];
        }

        public List<Player> Players { get => this.players; }

        // DECK
        public void ShuffleDeck(){
            this.deck.Shuffle();
        }

        // AUTODEAL
        public void Deal(){
            for (int i = 0; i < 2; i++){
                foreach (Player player in players){
                    player.Deal(deck.Draw(), 0);
                }
                this.dealer.Deal(deck.Draw(), (i == 0));
            }
            foreach (Player player in players){
                player.CheckSplit();
            }
        }

        // PLAYER MOVES
        public bool BetPlayer(Player player, int index, int amount){
            return player.MakeBet(index, amount);
        }

        public void SplitPlayer(Player player){
            player.SplitHand();

            int handNr = player.Hands.Count - 1;
            BetPlayer(player, handNr, player[0].Bet);

            player.Deal(deck.Draw(), 0);
            player.Deal(deck.Draw(), handNr);
            player.CheckSplit();
        }

        public bool DoubledownPlayer(Player player, int index){
            Hand hand = player[index];
            if (this.BetPlayer(player, index, hand.Bet))
            {
                HitPlayer(player, index);
                if (hand.Outcome == Hand.OutcomeHand.NONE)
                {
                    hand.Stand = true;
                }
                return true;
            }
            return false;
        }

        // PLAYER/DEALER MOVES
        public void HitPlayer(Player player, int index){
            player.Deal(this.deck.Draw(), index);
        }

        public void StandPlayer(Player player, int index){
            player.Stand(index);
        }

        public void HitDealer(){
            this.dealer.Deal(this.deck.Draw());
        }

        public void StandDealer(){
            this.dealer.Stand();
        }


        // DISCARD
        public void DiscardAll(){
            foreach (Player player in players){
                this.DiscardPlayer(player);
            }
            this.DiscardDealer();
        }

        public void DiscardPlayer(Player player){
            foreach (Hand hand in player.Hands){
                foreach (Card card in hand.Cards){
                    this.discard.Add(card);
                }
            }
            player.Discard();
        }

        public void DiscardDealer(){
            foreach (Card card in this.dealer.Hand.Cards){
                this.discard.Add(card);
            }
            this.dealer.Discard();
        }

        // REFILL
        public void Refill(){
            foreach (Card card in this.discard.Cards){
                this.deck.Add(card);
            }
            this.discard.Clear();
        }

        // DISPLAY
        public override string ToString(){
            string output = "";
            output += this.deck.ToString() + "\n";
            output += this.discard.ToString() + "\n";
            output += this.dealer.ToString();
            foreach (Player player in players){
                output += player.ToString() + "\n";
            }
            return output;
        }
    }
}
