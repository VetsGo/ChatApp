using Microsoft.AspNetCore.SignalR;
using ChatApp.Backend.Models;
using ChatApp.Backend.Data;
using ChatApp.Backend.Services;

namespace ChatApp.Backend.Hubs
{
    // SignalR Hub for real-time message exchange
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _dbContext;
        private readonly ILogger<ChatHub> _logger;
        private readonly ISentimentAnalysisService _sentimentService;

        public ChatHub(
            ChatDbContext dbContext, 
            ILogger<ChatHub> logger,
            ISentimentAnalysisService sentimentService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _sentimentService = sentimentService;
        }

        // Receiving a message from the client, performing sentiment analysis, and saving data to the database
        public async Task SendMessage(string username, string message)
        {
            try
            {
                var sentimentResult = await _sentimentService.AnalyzeSentimentAsync(message);

                var chatMessage = new ChatMessage
                {
                    Username = username,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    Sentiment = sentimentResult.Sentiment,
                    PositiveScore = sentimentResult.PositiveScore,
                    NegativeScore = sentimentResult.NegativeScore,
                    NeutralScore = sentimentResult.NeutralScore
                };

                _dbContext.ChatMessages.Add(chatMessage);
                await _dbContext.SaveChangesAsync();

                await Clients.All.SendAsync("ReceiveMessage", new
                {
                    id = chatMessage.Id,
                    username = chatMessage.Username,
                    message = chatMessage.Message,
                    timestamp = chatMessage.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                    sentiment = chatMessage.Sentiment,
                    positiveScore = chatMessage.PositiveScore,
                    negativeScore = chatMessage.NegativeScore,
                    neutralScore = chatMessage.NeutralScore
                });

                _logger.LogInformation($"Message sent by {username} with sentiment: {chatMessage.Sentiment}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw;
            }
        }

        // Logging client connection
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        // Logging client disconnection
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}