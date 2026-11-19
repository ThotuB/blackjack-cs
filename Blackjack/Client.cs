using System;

namespace Blackjack
{
    static class Client{
        // INPUT
        private static String ReadInput(String prompt){
            Console.Write(prompt);
            String input = Console.ReadLine();
            if (input == null){
                Environment.Exit(0);
            }
            return input.Trim();
        }

        // BETS
        public static void PlaceBet(Game game, Player player){
            if (player.Money < 2){
                Console.WriteLine(player.Name + " is out of money.");
                return;
            }

            while (true){
                String input = ReadInput(player.Name + " (money " + player.Money + "), place an even bet: ");
                if (int.TryParse(input, out int amount) && amount > 0 && game.BetPlayer(player, 0, amount)){
                    return;
                }
                Console.WriteLine("Invalid bet.");
            }
        }

        // PLAYER'S TURN
        public static void ManualDeal(Game game, Player player){
            // Splitting adds hands, so Hands.Count can grow while looping
            for (int i = 0; i < player.Hands.Count; i++){
                Hand hand = player[i];
                while (hand.Outcome == Hand.OutcomeHand.NONE){
                    bool canDouble = hand.Cards.Count == 2 && player.Money >= hand.Bet;
                    bool canSplit = player.Split && player.Hands.Count == 1 && player.Money >= hand.Bet;

                    Console.WriteLine();
                    Console.WriteLine("Dealer: " + Show(game.Dealer.Hand));
                    Console.WriteLine(player.Name + " hand " + (i + 1) + ": " + Show(hand) + " bet " + hand.Bet);

                    String options = "(H)it, (S)tand";
                    if (canDouble){
                        options += ", (D)ouble down";
                    }
                    if (canSplit){
                        options += ", s(P)lit";
                    }

                    String input = ReadInput(options + ": ").ToUpper();
                    if (input == "H"){
                        game.HitPlayer(player, i);
                    }
                    else if (input == "S"){
                        game.StandPlayer(player, i);
                    }
                    else if (input == "D" && canDouble){
                        game.DoubledownPlayer(player, i);
                    }
                    else if (input == "P" && canSplit){
                        game.SplitPlayer(player);
                    }
                }
                Console.WriteLine(player.Name + " hand " + (i + 1) + ": " + Show(hand) + " " + hand.Outcome);
            }
        }

        // DEALER'S TURN
        public static void AutoDeal(Game game, int threshold){
            Hand hand = game.Dealer.Hand;
            while (hand.Outcome == Hand.OutcomeHand.NONE && hand.Points < threshold){
                game.HitDealer();
            }
            if (hand.Outcome == Hand.OutcomeHand.NONE){
                game.StandDealer();
            }
            Console.WriteLine();
            Console.WriteLine("Dealer: " + Show(hand) + " " + hand.Outcome);
        }

        // PAYOUT
        public static void Payout(Player player, Dealer dealer){
            Hand dealerHand = dealer.Hand;
            bool dealerNatural = dealerHand.Cards.Count == 2 && dealerHand.Points == 21;

            for (int i = 0; i < player.Hands.Count; i++){
                Hand hand = player[i];
                bool natural = player.Hands.Count == 1 && hand.Cards.Count == 2 && hand.Points == 21;

                // Bets are already taken from the player's money, so 1 returns the bet
                double multiplier;
                if (hand.Bust){
                    multiplier = 0;
                }
                else if (natural && !dealerNatural){
                    multiplier = 2.5;
                }
                else if (dealerNatural && !natural){
                    multiplier = 0;
                }
                else if (dealerHand.Bust || hand.Points > dealerHand.Points){
                    multiplier = 2;
                }
                else if (hand.Points == dealerHand.Points){
                    multiplier = 1;
                }
                else{
                    multiplier = 0;
                }

                player.PayoutHand(hand, multiplier);
                Console.WriteLine(player.Name + " hand " + (i + 1) + ": " + Result(multiplier) + " (money " + player.Money + ")");
            }
        }

        // DISPLAY
        private static String Show(Hand hand){
            String result = "";
            foreach (Card card in hand.Cards){
                result += (card.Hidden ? "??" : card.ToString()) + " ";
            }
            return result + "(" + hand.Points + ")";
        }

        private static String Result(double multiplier){
            if (multiplier == 0){
                return "lose";
            }
            if (multiplier == 1){
                return "push";
            }
            if (multiplier == 2.5){
                return "blackjack";
            }
            return "win";
        }
    }
}
