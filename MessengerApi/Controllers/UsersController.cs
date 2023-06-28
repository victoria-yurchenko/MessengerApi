using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Messenger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Messenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private TableClient _tableClient;
        private BlobContainerClient _containerClient;

        public UsersController(TableClient tableClient, BlobContainerClient containerClient)
        {
            _tableClient = tableClient;
            _containerClient = containerClient;
        }

        [HttpGet]
        public IEnumerable<ChatUser> Index()
        {
            var users = _tableClient.Query<ChatUser>(u => u.PartitionKey == "User"); //select * from table
            return users.ToList();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(ChatUser user)
        {
            var registerUser = _tableClient.Query<ChatUser>(u =>
                   u.Email == user.Email ||
                   u.Name == user.Name
               )
               .FirstOrDefault();

            // such user is already exist
            if (registerUser != null)
                return BadRequest($"{registerUser} is already exist");

            // there is no such user
            var bytes = Convert.FromBase64String(user.Avatar);
            var blobName = $"{user.Name}.jpg";
            var blobClient = _containerClient.GetBlobClient(blobName);
            using (var ms = new MemoryStream(bytes)) 
                blobClient.Upload(ms);
            user.Timestamp = DateTimeOffset.UtcNow;
            user.Avatar = blobClient.Uri.AbsoluteUri;
            await _tableClient.AddEntityAsync(user);
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(ChatUser user)
        {
            var loginUser = _tableClient.Query<ChatUser>(
                    u => u.Email == user.Email &&
                    u.Password == user.Password)
                .FirstOrDefault();

            if (loginUser == null)
                return NotFound();
            else
            {
                HttpContext.Session.SetString("Logged", loginUser.RowKey);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
