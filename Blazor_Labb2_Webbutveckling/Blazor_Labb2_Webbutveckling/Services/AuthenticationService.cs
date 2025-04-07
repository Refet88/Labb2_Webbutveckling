using Blazor_Labb2_Webbutveckling.Models;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Blazor_Labb2_Webbutveckling.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public AuthenticationService(IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime)
        {
            _httpClient = httpClientFactory.CreateClient("AuthorizedClient");
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginRequest = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var tokenObject = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (tokenObject != null && tokenObject.ContainsKey("token"))
                {
                    await SaveToken(tokenObject["token"]);
                    return true;
                }
            }

            return false;
        }

        public async Task SaveToken(string token)
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "authToken", token);
        }

        public async Task<string?> GetToken()
        {
            return await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", "authToken");
        }

        public string GetRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                return string.Empty;
            }

            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        }

        public int? GetCustomerIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var customerIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "customerId")?.Value;

            return int.TryParse(customerIdClaim, out var customerId) ? customerId : null;
        }

        public async Task<HttpClient> GetAuthorizedHttpClient()
        {
            var token = await GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return _httpClient;
        }

        public async Task<int> GetCurrentUserId()
        {
            var httpClient = await GetAuthorizedHttpClient();
            var token = httpClient.DefaultRequestHeaders.Authorization?.Parameter;

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("No token found.");
            }

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var customerIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "customerId");

            if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out var customerId))
            {
                throw new FormatException($"Invalid Customer ID format: {customerIdClaim?.Value}");
            }

            return customerId;
        }

        public string GetNameFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                return string.Empty;
            }

            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
        }

        [JSInvokable]
        public async Task LogoutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "authToken");
        }

        public async Task ClearToken()
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "authToken");
        }
    }
}