using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatApp.Backend.Data;
using ChatApp.Backend.Models;

namespace ChatApp.Backend.Controllers
{
    // REST API controller for managing messages
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatDbContext _dbContext;

        public ChatController(ChatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Returns all messages sorted by send time
        [HttpGet("messages")]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetMessages(
            [FromQuery] int limit = 50)
        {
            var messages = await _dbContext.ChatMessages
                .OrderByDescending(m => m.Timestamp)
                .Take(limit)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return Ok(messages);
        }
 
        // Returns a specific message by ID
        [HttpGet("messages/{id}")]
        public async Task<ActionResult<ChatMessage>> GetMessage(int id)
        {
            var message = await _dbContext.ChatMessages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // Service health check
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}