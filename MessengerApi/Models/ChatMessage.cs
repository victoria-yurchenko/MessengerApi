using Azure;
using Azure.Data.Tables;

namespace Messenger.Models
{
    public class ChatMessage : ITableEntity
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string DataUrl { get; set; }
        public string DataType { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }


        public ChatMessage()
        {
            PartitionKey = "Message";
            RowKey = Guid.NewGuid().ToString();
            ETag = new ETag();
            Timestamp = DateTime.Now;
        }

        public ChatMessage(string from, string to, string message, string dataUrl, string dataType) : this()
        {
            From = from;
            To = to;
            Message = message;
            DataUrl = dataUrl;
            DataType = dataType;
        }
    }
}
