using Azure.Data.Tables;
using Azure.Storage.Blobs;

namespace MessengerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var connectionString = configuration["StorageConnectionString"];
            var accountName = configuration.GetSection("AppSettings")["AccountName"];
            var accountKey = configuration.GetSection("AppSettings")["AccountKey"];
            var tableName = configuration.GetSection("AppSettings")["TableName"];
            var blobContainerName = configuration.GetSection("AppSettings")["BlobContainerName"];
            var storageUrl = configuration.GetSection("AppSettings")["StorageUrl"];

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

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
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
            app.UseCors(MyAllowSpecificOrigins);
            app.MapControllers();
            app.Run();
        }
    }
}