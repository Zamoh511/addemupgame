using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DocumentFormat.OpenXml.Vml;
using iTextSharp.text.pdf.parser;

namespace addemupgame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define a List to hold the player information
            List<Player> players = new List<Player>();

          string line;
            string outputFilePath = "C:\\Output\\xyz.txt";
            string inputFilePath = "C:\\Output\\abc.txt";
            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                // Read each line of the input file and process the player information
                while ((line = reader.ReadLine()) != null)
                {
                    string[] extractName = line.Split(':');
                    string[] values = extractName[1].Split(',');

                    // Extract the player name from the input line
                    string playerName = extractName[0];

                    // Extract the card values from the input line
                    List<Card> cards = new List<Card>();
                    for (int i = 1; i < values.Length; i++)
                    {
                        string[] cardValues = values[i].Split(',');
                        //int value =Convert.ToInt32 (cardValues[0].Remove(cardValues[0].Length-1));
                        int value = isInt(Convert.ToString(cardValues[0]));
                        string suit = cardValues[0].Substring(1,1);
                        Card card = new Card(value, suit);
                        cards.Add(card);
                    }
                    //cards.Add(card);
                    // Create a new player object and add it to the list
                    var player = new Player(cards);
                    player.PlayerName = playerName;
                    players.Add(player);
                }
            }

            // Calculate the base score and suit score for each player
            foreach (Player player in players)
            {
                int baseScore = 0;
                List<Card> sortedCards = player.Cards.OrderByDescending(c => c.Value).ToList();
                for (int i = 0; i < 3; i++)
                {
                    baseScore += sortedCards[i].Value;
                }
                player.BaseScore = baseScore;

                int suitScore = 0;
                Card highestCard = sortedCards[0];
                switch (highestCard.Suit)
                {
                    case "diamonds":
                        suitScore = 1;
                        break;
                    case "hearts":
                        suitScore = 2;
                        break;
                    case "spades":
                        suitScore = 3;
                        break;
                    case "clubs":
                        suitScore = 4;
                        break;
                }
                player.SuitScore = suitScore;
            }

            // Sort the players based on their total score (base score + suit score)
            List<Player> sortedPlayers = players.OrderByDescending(p => p.BaseScore + p.SuitScore).ToList();

            // Find the highest score
            int highestScore = sortedPlayers[0].BaseScore + sortedPlayers[0].SuitScore;

            // Find all players with the highest score
            List<Player> winners = sortedPlayers.Where(p => p.BaseScore + p.SuitScore == highestScore).ToList();

            // Open the output file for writing
            using StreamWriter writer = new StreamWriter(outputFilePath);

            // Write the winner(s) to the output file
            if (winners.Count == 1)
            {
                writer.WriteLine($"{winners[0].PlayerName}:{winners[0].TotalScore}");
                //writer.WriteLine($":{winners[0].TotalScore}");
            }
            else
            {
                string winnerNames = string.Join(", ", winners.Select(p => p));
                writer.WriteLine($"Tie between: {winnerNames}");
            }



        }
        public static int isInt(string  val)
        {
            val = val.Substring(0,1);
            int value = 0;
            switch (val.ToUpper().Trim())
            {
                case "A":
                    value = 11;
                    break;
                case "J":
                    value = 11;
                    break;
                case "Q":
                    value = 12;
                    break;
                case "K":
                    value = 13;
                    break;
            }
            value = Convert.ToInt32(value);
            return value;
        }
        public class Card
        {
            public int Value { get; set; }
            public string Suit { get; set; }
            public int Score { get; set; }

            // Constructor to initialize the card object
            public Card(int value, string suit)
            {
                Value = value;
                Suit = suit;

                // Set the score of the card based on its suit
                

                 switch (suit)
                {
                    case "diamonds":
                        Score = 1;
                        break;
                    case "hearts":
                        Score = 2;
                        break;
                    case "spades":
                        Score = 3;
                        break;
                    case "clubs":
                        Score = 4;
                        break;
                }

            }
        }

        public class Player
        {
            public List<Card> Cards { get; set; }
            public int BaseScore { get; set; }
            public int SuitScore { get; set; }
            public int TotalScore { get; set; }
            public string PlayerName { get; set; }

            // Constructor to initialize the player object
            public Player(List<Card> cards)
            {
                Cards = cards;

                // Calculate the base score for the player
                BaseScore = cards.OrderByDescending(c => c.Value)
                                .Take(3)
                                .Sum(c => c.Value);

                // Calculate the suit score for the player
                SuitScore = cards.Max(c => c.Score);

                // Calculate the total score for the player
                TotalScore = BaseScore + SuitScore;
            }
        }

    }
}
