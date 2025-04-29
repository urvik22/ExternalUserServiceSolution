using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ExternalUserService.Configuration;
using ExternalUserService.Services;
using ExternalUserService.Models;

public class ExternalUserServiceTests
{
    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenApiSuccess()
    {
        // Arrange
        var mockHttpHandler = new Mock<HttpMessageHandler>();
        mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"data\":{\"id\":1,\"email\":\"test@test.com\",\"first_name\":\"John\",\"last_name\":\"Doe\",\"avatar\":\"url\"}}")
            });

        var httpClient = new HttpClient(mockHttpHandler.Object);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var loggerMock = new Mock<ILogger<ExternalUserService.Services.ExternalUserService>>();
        var options = Options.Create(new ApiSettings
        {
            BaseUrl = "https://reqres.in/api",
            CacheDurationMinutes = 5
        });

        var service = new ExternalUserService.Services.ExternalUserService(httpClient, memoryCache, loggerMock.Object, options);

        // Act
        var user = await service.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
        Assert.Equal("John", user.First_Name);
        Assert.Equal("Doe", user.Last_Name);
    }
}
