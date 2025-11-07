using System.ComponentModel.DataAnnotations;

namespace ChatApp.Backend.Models
{
    // Chat message model for storing data in the database
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? Sentiment { get; set; }
        
        public double? PositiveScore { get; set; } 
        
        public double? NegativeScore { get; set; }
        
        public double? NeutralScore { get; set; }
    }
}