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
            var messages = _tableClient.Query<ChatMessage>(m => m.PartitionKey == "Message" && (m.To == "" || m.To == logged ));
            //var messages = _tableClient.Query<ChatMessage>(m => m.PartitionKey == "Message");

            return messages.ToList();
        }

        [HttpPost]
        [RequestSizeLimit(5000000)]
        public async Task<IActionResult> SendMessageAsync(ChatMessage message)
        {
            try
            {
                var bytes = Convert.FromBase64String(message.DataUrl);
                var blobName = $"{message.RowKey}.{message.DataType}";

                var blobClient = _containerClient.GetBlobClient(blobName);

                using (var ms = new MemoryStream(bytes))
                    blobClient.Upload(ms);
                
                message.DataUrl = blobClient.Uri.AbsoluteUri;

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
