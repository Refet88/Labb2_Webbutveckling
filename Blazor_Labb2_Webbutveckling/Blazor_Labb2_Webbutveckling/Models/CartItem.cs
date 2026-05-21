namespace Blazor_Labb2_Webbutveckling.Models;

public class CartItem
{
    public int ProductNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsDiscontinued { get; set; }
    public int Quantity { get; set; }

    public decimal LineTotal => Price * Quantity;

    public static CartItem FromProduct(Product product, int quantity) => new()
    {
        ProductNumber = product.ProductNumber,
        Name = product.Name ?? string.Empty,
        Price = product.Price,
        StockQuantity = product.StockQuantity,
        IsDiscontinued = product.IsDiscontinued,
        Quantity = quantity
    };
}
