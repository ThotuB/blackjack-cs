using System;
using System.Collections.Generic;

namespace Blackjack
{
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

        public int Money { get => this.money; }

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
            if ( hand.Cards.Count == 2 && hand[0].Value == hand[1].Value ){
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
}
