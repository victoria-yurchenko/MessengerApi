using Azure;
using Azure.Data.Tables;

namespace Messenger.Models
{
    public class ChatUser : ITableEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }


        public ChatUser()
        {
            PartitionKey = "User";
            RowKey = Guid.NewGuid().ToString();
            ETag = new ETag();
            Timestamp = DateTime.Now;
        }

        public ChatUser(string name, string email, string password, string avatar) : this()
        {
            Name = name;
            Email = email;
            Password = password;
            Avatar = avatar;
        }
    }
}
