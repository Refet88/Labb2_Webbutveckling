using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Labb2_Webbutveckling.Data;
using Labb2_Webbutveckling.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly ECommerceDbContext _context;

    public ProductRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync() ?? new List<Product>();
    }

    public async Task<Product?> GetProductByProductNumberAsync(int productNumber)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.ProductNumber == productNumber);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await _context.Products.ToListAsync();
        }

        return await _context.Products
            .Where(p => p.Name!.Contains(query) || p.ProductNumber.ToString().Contains(query))
            .ToListAsync() ?? new List<Product>();
    }
    public async Task AddProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateProductAsync(int productNumber, Product updatedProduct)
    {
        var existingProduct = await GetProductByProductNumberAsync(productNumber);
        if (existingProduct == null)
        {
            return false;
        }

        existingProduct.Name = updatedProduct.Name;
        existingProduct.Description = updatedProduct.Description;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.Category = updatedProduct.Category;
        existingProduct.ProductNumber = updatedProduct.ProductNumber;
        existingProduct.StockQuantity = updatedProduct.StockQuantity;
        existingProduct.IsDiscontinued = updatedProduct.IsDiscontinued;

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task DiscontinueProductAsync(int productNumber)
    {
        var product = await GetProductByProductNumberAsync(productNumber);
        if (product != null)
        {
            product.IsDiscontinued = true;
            await _context.SaveChangesAsync();
        }
    }
    public async Task ReactivateProductAsync(int productNumber)
    {
        var product = await GetProductByProductNumberAsync(productNumber);
        if (product != null)
        {
            product.IsDiscontinued = false;
            await _context.SaveChangesAsync();
        }
    }
    public async Task DeleteProductAsync(int productNumber)
    {
        var product = await GetProductByProductNumberAsync(productNumber);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UpdateStockAsync(int productNumber, int quantity)
    {
        var product = await _context.Products.FindAsync(productNumber);
        if (product == null) return false;

        product.StockQuantity += quantity;

        if (product.StockQuantity < 0)
        {
            return false;
        }

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return true;
    }


}