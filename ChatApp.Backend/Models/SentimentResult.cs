namespace ChatApp.Backend.Models
{
    // For sentiment analysis results
    public class SentimentResult
    {
        public string Sentiment { get; set; } = "Neutral";
        public double PositiveScore { get; set; }
        public double NegativeScore { get; set; }
        public double NeutralScore { get; set; }
    }
}