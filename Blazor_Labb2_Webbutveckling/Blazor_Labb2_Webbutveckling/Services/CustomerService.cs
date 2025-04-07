using Blazor_Labb2_Webbutveckling.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Blazor_Labb2_Webbutveckling.Services
{
    public class CustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationService _authService;

        public CustomerService(IHttpClientFactory httpClientFactory, AuthenticationService authService)
        {
            _httpClient = httpClientFactory.CreateClient("AuthorizedClient");
            _authService = authService;
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.GetAsync("api/customers");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Customer>>() ?? new List<Customer>();
        }

        public async Task<Customer?> GetCustomerById(int customerId)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.GetAsync($"api/customers/{customerId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Customer>();
        }

        public async Task<Customer> RegisterCustomer(Customer customer)
        {
            var response = await _httpClient.PostAsJsonAsync("api/customers/register", customer);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You do not have the required permissions to perform this action.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Customer>() ?? new Customer();
        }

        public async Task UpdateCustomer(Customer customer)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.PutAsJsonAsync($"api/customers/{customer.CustomerId}", customer);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update customer: {error}");
            }
        }

        public async Task DeleteCustomer(int id)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await _httpClient.DeleteAsync($"api/customers/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You do not have the required permissions to perform this action.");
            }

            response.EnsureSuccessStatusCode();
        }

        public async Task<Customer?> SearchCustomerByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be empty or null.", nameof(email));
            }

            var httpClient = await _authService.GetAuthorizedHttpClient();
            var encodedEmail = Uri.EscapeDataString(email);
            var response = await httpClient.GetAsync($"api/customers/email/{encodedEmail}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You do not have the required permissions to access this resource.");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Customer>();
        }
    }
}