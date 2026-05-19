using Blazor_Labb2_Webbutveckling.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

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
            var payload = ProductUpdateDto.FromProduct(product);
            var response = await httpClient.PostAsJsonAsync("api/products", payload);

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
            var payload = ProductUpdateDto.FromProduct(product);
            var response = await httpClient.PutAsJsonAsync($"api/products/{product.ProductNumber}", payload);

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

        public async Task ApplyImageChangesAsync(
        int productNumber,
        IEnumerable<int> imageIdsToDelete,
        IEnumerable<IBrowserFile> filesToUpload,
        bool setFirstAsPrimary,
        int currentImageCount)
        {
            foreach (var imageId in imageIdsToDelete)
            {
                if (!await DeleteProductImageAsync(productNumber, imageId))
                    throw new HttpRequestException($"Could not delete image {imageId}.");
            }

            foreach (var file in filesToUpload)
            {
                await using var stream = file.OpenReadStream(maxAllowedSize: 5_000_000);
                var uploaded = await UploadProductImageAsync(productNumber, stream, file.Name);
                if (uploaded == null)
                    throw new HttpRequestException($"Could not upload image '{file.Name}'.");
            }
        }

        public async Task<List<ProductImage>> GetProductImagesAsync(int productNumber)
        {
            var response = await _httpClient.GetAsync($"api/products/{productNumber}/images");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ProductImage>>() ?? new List<ProductImage>();
        }

        public async Task<ProductImage?> UploadProductImageAsync(
            int productNumber,
            Stream fileStream,
            string fileName)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();

            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".avif" => "image/avif",
                _ => "image/jpeg"
            };
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(streamContent, "file", fileName);

            var response = await httpClient.PostAsync($"api/products/{productNumber}/images", content);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ProductImage>();
        }

        public async Task<bool> DeleteProductImageAsync(int productNumber, int imageId)
        {
            var httpClient = await _authService.GetAuthorizedHttpClient();
            var response = await httpClient.DeleteAsync($"api/products/{productNumber}/images/{imageId}");
            return response.IsSuccessStatusCode;
        }

        public class StockUpdateRequest
        {
            public int QuantityChange { get; set; }
        }
    }
}