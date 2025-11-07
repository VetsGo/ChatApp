using ChatApp.Backend.Models;

namespace ChatApp.Backend.Services
{
    // Interface for the sentiment analysis service
    public interface ISentimentAnalysisService
    {
        Task<SentimentResult> AnalyzeSentimentAsync(string text);
    }
}