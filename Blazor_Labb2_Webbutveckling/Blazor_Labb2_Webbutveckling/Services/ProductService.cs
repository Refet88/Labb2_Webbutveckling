using Blazor_Labb2_Webbutveckling.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Blazor_Labb2_Webbutveckling.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationService _authService;

        public ProductService(IHttpClientFactory httpClientFactory, AuthenticationService authService)
        {
            _httpClient = httpClientFactory.CreateClient("AuthorizedClient");
            _authService = authService;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var response = await _httpClient.GetAsync("api/Products");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this resource.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Product>>() ?? new List<Product>();
        }

        public async Task<Product> GetProductById(int id)
        {
            var response = await _httpClient.GetAsync($"api/products/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this resource.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Product>() ?? new Product();
        }

        public async Task<bool> AddProduct(Product product)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.PostAsJsonAsync("api/products", product);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to add a product.");
            }

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        public async Task UpdateProduct(Product product)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.PutAsJsonAsync($"api/products/{product.ProductNumber}", product);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this resource.");
            }

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProduct(int id)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.DeleteAsync($"api/products/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this resource.");
            }

            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Product>> SearchProducts(string query)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.GetAsync($"api/products/search/{Uri.EscapeDataString(query)}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to search for products.");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Product>>() ?? Enumerable.Empty<Product>();
        }

        public async Task DiscontinueProduct(int id)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.PutAsJsonAsync($"api/products/discontinue/{id}", new { });

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to discontinue this product.");
            }

            response.EnsureSuccessStatusCode();
        }

        public async Task ReactivateProduct(int id)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.PutAsJsonAsync($"api/products/reactivate/{id}", new { });

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to reactivate this product.");
            }

            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> UpdateStockAsync(int productNumber, int quantity)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var payload = new StockUpdateRequest { QuantityChange = quantity };
            var response = await httpClient.PutAsJsonAsync($"api/products/{productNumber}/update-stock", payload);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You are not authorized to update stock.");
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        public class StockUpdateRequest
        {
            public int QuantityChange { get; set; }
        }
    }
}