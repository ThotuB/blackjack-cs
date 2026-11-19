using System;
using System.Collections.Generic;

namespace Blackjack
{
    class Program
    {
        public static void Main(String[] args)
        {
            List<Player> playerArr = new List<Player>();
            playerArr.Add(new Player("42324", "Thotu", 100));
            playerArr.Add(new Player("43324", "Fake Thotu", 100));

            Game game = new Game(playerArr);

            game.ShuffleDeck();
            Console.WriteLine(game);

            bool goNext;

            do
            {
                // BETS
                foreach (Player player in game.Players)
                {
                    Client.PlaceBet(game, player);
                }

                // ROUND START
                game.Deal();

                // PLAYERS' TURNS
                foreach (Player player in game.Players)
                {
                    Client.ManualDeal(game, player);
                }

                // DEALER'S TURN
                Dealer dealer = game.Dealer;

                dealer.ShowCard(0);

                Client.AutoDeal(game, 17);

                // PAYOUT
                foreach (Player player in game.Players)
                {
                    Client.Payout(player, dealer);
                }

                // ROUND END
                game.DiscardAll();
                game.Refill();

                Console.Write("Another round? (Y): ");
                goNext = "Y".Equals(Console.ReadLine(), StringComparison.OrdinalIgnoreCase);
            } while (goNext);
        }
    }
}
