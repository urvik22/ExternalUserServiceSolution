using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ExternalUserService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ExternalUserService.Configuration;

namespace ExternalUserService.Services
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ExternalUserService> _logger;
        private readonly ApiSettings _apiSettings;

        public ExternalUserService(HttpClient httpClient, IMemoryCache cache, ILogger<ExternalUserService> logger, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _apiSettings = apiSettings.Value;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            string cacheKey = $"User_{userId}";
            if (_cache.TryGetValue(cacheKey, out User cachedUser))
                return cachedUser;

            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to fetch user with ID {userId}. Status Code: {response.StatusCode}");
                throw new HttpRequestException($"Unable to fetch user {userId}, status: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(content);
            var dataElement = document.RootElement.GetProperty("data");

            var user = JsonSerializer.Deserialize<User>(dataElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (user != null)
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(_apiSettings.CacheDurationMinutes));

            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            string cacheKey = "AllUsers";
            if (_cache.TryGetValue(cacheKey, out List<User> cachedUsers))
                return cachedUsers;

            var users = new List<User>();
            int page = 1;
            int totalPages = 1; // default 1

            do
            {
                var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/users?page={page}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch users on page {page}. Status Code: {response.StatusCode}");
                    break;
                }

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse?.Data != null)
                {
                    users.AddRange(apiResponse.Data);
                    totalPages = apiResponse.TotalPages;
                    page++;
                }
                else
                {
                    break;
                }

            } while (page <= totalPages);

            _cache.Set(cacheKey, users, TimeSpan.FromMinutes(_apiSettings.CacheDurationMinutes));
            return users;
        }
    }
}
