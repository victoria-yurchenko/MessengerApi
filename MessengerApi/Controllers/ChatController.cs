using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Messenger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Messenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private TableClient _tableClient;
        private BlobContainerClient _containerClient;

        public ChatController(TableClient tableClient, BlobContainerClient containerClient)
        {
            _tableClient = tableClient;
            _containerClient = containerClient;
        }


        [HttpGet]
        public IEnumerable<ChatMessage> GetMessages()
        {
            var logged = HttpContext.Session.GetString("Logged") ?? string.Empty;
            var messages = _tableClient.Query<ChatMessage>(m => m.PartitionKey == "Message" && m.To == "");

            return messages.ToList();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageAsync(ChatMessage message)
        {
            try
            {
                await _tableClient.AddEntityAsync(message);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
