using System;
using System.Collections.Generic;

namespace Blackjack
{
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
            Random rnd = new Random();
            for (int indexCurr = this.size; indexCurr != 0; indexCurr--){
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
}
