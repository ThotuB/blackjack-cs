using System;
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

    /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    class Dealer{
        private Hand hand;

        public Dealer(){
            this.hand = new Hand();
        }

        // GETTERS
        public Hand Hand { get; }

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

    /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    class Player{
        private String id;
        private String name;
        private int money;
        private int totalBet;

        private List<Hand> hands;

        private bool split;

        public Player(String id, String name, int money){
            this.id = id;
            this.name = name;
            this.money = money;

            this.hands = new List<Hand>(4);
            this.hands.Add(new Hand());

            this.split = false;
        }

        // GETTERS
        public String Name { get => this.name; }

        public Hand this[int index] {
            get => this.hands[index];
        }

        public List<Hand> Hands { get => this.hands; }

        public bool Split { get => this.split; }

        // BETS
        public bool MakeBet(int index, int amount){
            if (0 <= amount && amount <= this.money && amount % 2 == 0){
                this.hands[index].MakeBet(amount);
                this.money -= amount;
                this.totalBet += amount;

                return true;
            }
            return false;
        }

        // SPLIT
        public void CheckSplit(){
            Hand hand = this.hands[0];
            if ( hand[0].Value == hand[0].Value ){
                this.split = true;
            }
            else{
                this.split = false;
            }
        }

        public void SplitHand(){
            Card card = this.hands[0].Remove(1);
            Hand newHand = new Hand();

            newHand.Add(card, false);
            this.hands.Add(newHand);
        }

        // MOVES
        public void Deal(Card card, int index, bool hidden){
            this.hands[index].Add(card, hidden);
        }

        public void Deal(Card card, int index){
            this.Deal(card, index, false);
        }

        public void Stand(int index){
            this.hands[index].Stand = true;
        }

        // HIDE/SHOW CARD
        public void HideCardInHand(int indexCard, int indexHand){
            this.hands[indexHand].HideCard(indexCard);
        }

        public void ShowCardInHand(int indexCard, int indexHand){
            this.hands[indexHand].ShowCard(indexCard);
        }

        // PAYOUT
        public void PayoutHand(Hand hand, double multiplier){
            this.money += (int)Math.Round(hand.Bet * multiplier);
        }

        // DISCARD
        public void Discard(){
            foreach (Hand hand in this.hands){
                hand.Discard();
            }
            this.totalBet = 0;

            this.hands.Clear();
            this.hands.Add(new Hand());
            this.split = false;
        }

        // DISPLAY
        public override string ToString(){
            string result = "";
            result += "Player: " + this.name + "\n";
            result += "Money: " + this.money + "\n";
            result += "Total Bet: " + this.totalBet + "\n";
            result += "Hands: " + this.hands.Count + "\n";
            result += "Split: " + this.split + "\n";
            result += "Hand: " + this.hands[0].ToString() + "\n";

            return result;
        }
    }

    /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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

        public OutcomeHand Outcome { get; set; }

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

    /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    class Deck{
        private int size;
        private List<Card> cards;

        public Deck(int nrDecks){
            this.cards = new List<Card>();
            this.Reset(nrDecks);
        }

        // GETTERS
        public int Size { get => this.size;  }

        public List<Card> Cards { get => this.cards; }

        // AUTO FUNCTIONS
        public void Reset(int nrDecks){
            this.cards.Clear();
            for (int k = 0; k < nrDecks; k++){
                for (int i = 0; i < 4; i++){
                    for (int j = 0; j < 13; j++){
                        this.cards.Add(new Card(i, j));
                    }
                }
            }
            this.size = this.cards.Count;
        }

        public void Shuffle(){
            for (int indexCurr = this.size; indexCurr != 0; indexCurr--){
                Random rnd = new Random();
                int indexRand = rnd.Next(0, indexCurr);

                Card tempCard = this.cards[indexCurr - 1];
                this.cards[indexCurr - 1] = this.cards[indexRand];
                this.cards[indexRand] = tempCard;
            }
        }

        public Card Draw(){
            Card card = this.cards[0];

            this.cards.RemoveAt(0);
            this.size = this.cards.Count;

            return card;
        }

        public void Add(Card card){
            this.cards.Add(card);
            this.size = this.cards.Count;
        }

        public void Clear(){
            this.cards.Clear();
            this.size = 0;
        }

        // DISPLAY
        public override string ToString(){
            string result = "Deck:\n";
            foreach (Card card in this.cards){
                result += card.ToString() + "\n";
            }
            return result;
        }
    }

    /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    class Card{
        private String[] SUITS = {"\u2660", "\u2665", "\u2666", "\u2663"};
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

    /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    class Program{
        public static void Main(String[] args){
            List<Player> playerArr = new List<Player>();
            playerArr.Add(new Player("42324", "Thotu", 100));
            playerArr.Add(new Player("43324", "Fake Thotu", 100));

            Game game = new Game(playerArr);

            game.ShuffleDeck();
            Console.WriteLine(game);

            bool goNext;

            do{
                // BETS
                foreach (Player player in game.Players){
                    // Client.PlaceBet(game, player);
                }

                // // ROUND START
                // game.Deal();

                // // PLAYERS' TURNS
                // foreach (Player player in game.getPlayers()){
                //     Client.ManualDeal(game, player);
                // }

                // // DEALER'S TURN
                // Dealer dealer = game.getDealer();

                // dealer.ShowCard(0);

                // Client.autodeal(game, 17);

                // // PAYOUT
                // foreach (Player player in game.getPlayers()){
                //     Client.payout(player, dealer);
                // }

                // // ROUND END
                // game.DiscardAll();
                // game.Refill();

                Console.Write("Another round? (Y): ");
                goNext = (Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase));
            } while (goNext);
        }
    }
}