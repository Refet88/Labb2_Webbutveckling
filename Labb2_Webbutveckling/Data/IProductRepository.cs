using Labb2_Webbutveckling.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labb2_Webbutveckling.Data
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();

        Task<Product?> GetProductByProductNumberAsync(int productNumber);

        Task<IEnumerable<Product>> SearchProductsAsync(string query);

        Task AddProductAsync(Product product);

        Task<bool> UpdateProductAsync(int productNumber, Product product);

        Task DiscontinueProductAsync(int productNumber);

        Task ReactivateProductAsync(int productNumber);

        Task DeleteProductAsync(int productNumber);

        Task<bool> UpdateStockAsync(int productNumber, int quantity);
    }
}