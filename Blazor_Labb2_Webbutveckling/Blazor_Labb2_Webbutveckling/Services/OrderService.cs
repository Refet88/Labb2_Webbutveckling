using Blazor_Labb2_Webbutveckling.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Blazor_Labb2_Webbutveckling.Models.Blazor_Labb2_Webbutveckling.Models;

namespace Blazor_Labb2_Webbutveckling.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationService _authService;

        public OrderService(IHttpClientFactory httpClientFactory, AuthenticationService authService)
        {
            _httpClient = httpClientFactory.CreateClient("AuthorizedClient");
            _authService = authService;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.GetAsync("api/orders");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Order>>() ?? new List<Order>();
        }

        public async Task<Order?> GetOrderById(int orderId)
        {
            try
            {
                var httpClient = await _authService.GetAuthorizedHttpClient();
                var response = await httpClient.GetAsync($"api/orders/{orderId}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("You are not authorized to access this resource.");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Order>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerId(int customerId)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.GetAsync($"api/orders/customer/{customerId}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to view orders for this customer.");
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<Order>();
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Order>>() ?? new List<Order>();
        }

        public async Task<IEnumerable<Order>> SearchOrdersByCustomerAsync(string query)
        {
            Console.WriteLine($"Sending request to API: api/orders/search/customer/{query}");

            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.GetAsync($"api/orders/search/customer/{query}");

            Console.WriteLine($"Response status code: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response content: {responseContent}");
                return await response.Content.ReadFromJsonAsync<IEnumerable<Order>>() ?? new List<Order>();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error response content: {errorContent}");
            return new List<Order>();
        }




        public async Task<HttpResponseMessage> AddOrder(OrderRequest orderRequest)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.PostAsJsonAsync("api/orders/create", orderRequest);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to add an order.");
            }

            return response;
        }

        public async Task DeleteOrder(int id)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.DeleteAsync($"api/orders/{id}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this resource.");
            }

            response.EnsureSuccessStatusCode();
        }
    }
}