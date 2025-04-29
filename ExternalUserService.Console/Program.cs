using ExternalUserService.Services;
using ExternalUserService.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<ApiSettings>(context.Configuration.GetSection("ApiSettings"));
        services.AddMemoryCache();
        services.AddHttpClient<IExternalUserService, ExternalUserService.Services.ExternalUserService>()
                .AddPolicyHandler(GetRetryPolicy());
        services.AddLogging();
    })
    .Build();

var service = host.Services.GetRequiredService<IExternalUserService>();

Console.WriteLine("Fetching all users...");
var users = await service.GetAllUsersAsync();
foreach (var user in users)
{
    Console.WriteLine($"{user.First_Name} {user.Last_Name} - {user.Email}");
}

Console.WriteLine("\nFetching single user...");
var singleUser = await service.GetUserByIdAsync(1);
Console.WriteLine($"{singleUser.First_Name} {singleUser.Last_Name} - {singleUser.Email}");

Console.ReadLine();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
