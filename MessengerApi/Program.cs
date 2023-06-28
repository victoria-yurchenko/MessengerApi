using Azure.Data.Tables;
using Azure.Storage.Blobs;

namespace MessengerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = string.Empty;
            var accountName = string.Empty;
            var accountKey = string.Empty;
            var tableName = string.Empty;
            var blobContainerName = string.Empty;
            var storageUrl = string.Empty;

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            connectionString = configuration["StorageConnectionString"];
            accountName = configuration.GetSection("AppSettings")["AccountName"];
            accountKey = configuration.GetSection("AppSettings")["AccountKey"];
            tableName = configuration.GetSection("AppSettings")["TableName"];
            blobContainerName = configuration.GetSection("AppSettings")["BlobContainerName"];
            storageUrl = configuration.GetSection("AppSettings")["StorageUrl"];

            builder.Services.AddSingleton(provider =>
            {
                var serviceClient = new TableServiceClient(
                    new Uri(storageUrl),
                    new TableSharedKeyCredential(accountName, accountKey)
                );
                var tableClient = serviceClient.GetTableClient(tableName);
                tableClient.CreateIfNotExists();
                return tableClient;
            });

            builder.Services.AddSingleton(provider =>
            {
                var serviceClient = new BlobServiceClient(connectionString);
                var containerClient = serviceClient.GetBlobContainerClient(blobContainerName);
                containerClient.CreateIfNotExists();
                return containerClient;
            });
            
            builder.Services.AddControllers();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();
            app.UseAuthorization();
            app.UseSession();
            app.MapControllers();

            app.Run();
        }
    }
}