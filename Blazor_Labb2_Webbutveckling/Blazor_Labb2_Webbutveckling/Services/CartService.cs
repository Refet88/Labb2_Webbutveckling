using System.Text.Json;
using Blazor_Labb2_Webbutveckling.Models;
using Microsoft.JSInterop;

namespace Blazor_Labb2_Webbutveckling.Services;

public class CartService
{
    private const string StorageKey = "shopCart";
    private readonly IJSRuntime _jsRuntime;
    private readonly List<CartItem> _items = new();
    private bool _loaded;

    public CartService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public event Action? Changed;

    public IReadOnlyList<CartItem> Items => _items;

    public int TotalItemCount => _items.Sum(i => i.Quantity);

    public decimal TotalPrice => _items.Sum(i => i.LineTotal);

    public bool IsEmpty => _items.Count == 0;

    public int GetQuantity(int productNumber) =>
        _items.FirstOrDefault(i => i.ProductNumber == productNumber)?.Quantity ?? 0;

    public async Task EnsureLoadedAsync()
    {
        if (_loaded)
        {
            return;
        }

        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", StorageKey);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var stored = JsonSerializer.Deserialize<List<CartItem>>(json);
                if (stored != null)
                {
                    _items.Clear();
                    _items.AddRange(stored.Where(i => i.Quantity > 0));
                }
            }
        }
        catch
        {
            _items.Clear();
        }

        _loaded = true;
    }

    public async Task<(bool Success, string? Error)> AddAsync(Product product, int quantity = 1)
    {
        await EnsureLoadedAsync();

        if (product.IsDiscontinued)
        {
            return (false, "Produkten är utgången.");
        }

        if (quantity < 1)
        {
            return (false, "Antal måste vara minst 1.");
        }

        var existing = _items.FirstOrDefault(i => i.ProductNumber == product.ProductNumber);
        var newQty = (existing?.Quantity ?? 0) + quantity;

        if (newQty > product.StockQuantity)
        {
            return (false, $"Max {product.StockQuantity} st i lager.");
        }

        if (existing != null)
        {
            existing.Quantity = newQty;
            existing.Price = product.Price;
            existing.Name = product.Name ?? existing.Name;
            existing.StockQuantity = product.StockQuantity;
            existing.IsDiscontinued = product.IsDiscontinued;
        }
        else
        {
            _items.Add(CartItem.FromProduct(product, quantity));
        }

        await SaveAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> IncrementAsync(Product product)
    {
        await EnsureLoadedAsync();
        var current = GetQuantity(product.ProductNumber);
        if (current == 0)
        {
            return await AddAsync(product, 1);
        }

        return await SetQuantityAsync(product.ProductNumber, current + 1);
    }

    public async Task<(bool Success, string? Error)> DecrementAsync(Product product)
    {
        await EnsureLoadedAsync();
        var current = GetQuantity(product.ProductNumber);
        if (current <= 1)
        {
            await RemoveAsync(product.ProductNumber);
            return (true, null);
        }

        return await SetQuantityAsync(product.ProductNumber, current - 1);
    }

    public async Task<(bool Success, string? Error)> SetQuantityAsync(int productNumber, int quantity)
    {
        await EnsureLoadedAsync();

        var item = _items.FirstOrDefault(i => i.ProductNumber == productNumber);
        if (item == null)
        {
            return (false, "Produkten finns inte i kundvagnen.");
        }

        if (quantity <= 0)
        {
            await RemoveAsync(productNumber);
            return (true, null);
        }

        if (item.IsDiscontinued)
        {
            return (false, "Produkten är utgången.");
        }

        if (quantity > item.StockQuantity)
        {
            return (false, $"Max {item.StockQuantity} st i lager.");
        }

        item.Quantity = quantity;
        await SaveAsync();
        return (true, null);
    }

    public async Task RemoveAsync(int productNumber)
    {
        await EnsureLoadedAsync();
        _items.RemoveAll(i => i.ProductNumber == productNumber);
        await SaveAsync();
    }

    public async Task ClearAsync()
    {
        _items.Clear();
        _loaded = true;
        try
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", StorageKey);
        }
        catch
        {
            // ignore when JS is unavailable
        }

        Changed?.Invoke();
    }

    private async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(_items);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", StorageKey, json);
        Changed?.Invoke();
    }
}
