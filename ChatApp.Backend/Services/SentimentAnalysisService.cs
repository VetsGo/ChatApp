using Azure;
using Azure.AI.TextAnalytics;
using ChatApp.Backend.Models;

namespace ChatApp.Backend.Services
{
    // Implementing the sentiment analysis service using Azure Language Service
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly TextAnalyticsClient _client;
        private readonly ILogger<SentimentAnalysisService> _logger;

        // Initializing the Language Service client
        public SentimentAnalysisService(
            IConfiguration configuration,
            ILogger<SentimentAnalysisService> logger)
        {
            var endpoint = configuration["CognitiveServices:Endpoint"];
            var apiKey = configuration["CognitiveServices:ApiKey"];
            
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Cognitive Services credentials not configured. Sentiment analysis will return neutral results.");
            }
            
            _client = new TextAnalyticsClient(
                new Uri(endpoint!), 
                new AzureKeyCredential(apiKey!));
            
            _logger = logger;
        }
 
        // Sending text to the Azure API and receiving positive, negative, and neutral scores
        public async Task<SentimentResult> AnalyzeSentimentAsync(string text)
        {
            try
            {
                _logger.LogInformation($"Analyzing sentiment for text: {text.Substring(0, Math.Min(50, text.Length))}...");
                
                var response = await _client.AnalyzeSentimentAsync(text);
                var sentiment = response.Value;

                var result = new SentimentResult
                {
                    Sentiment = sentiment.Sentiment.ToString(),
                    PositiveScore = sentiment.ConfidenceScores.Positive,
                    NegativeScore = sentiment.ConfidenceScores.Negative,
                    NeutralScore = sentiment.ConfidenceScores.Neutral
                };

                _logger.LogInformation($"Sentiment analysis result: {result.Sentiment} (Pos: {result.PositiveScore:P0}, Neg: {result.NegativeScore:P0}, Neu: {result.NeutralScore:P0})");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing sentiment.");
                return new SentimentResult
                {
                    Sentiment = "Neutral",
                    PositiveScore = 0,
                    NegativeScore = 0,
                    NeutralScore = 1.0
                };
            }
        }
    }
}