namespace Blazor_Labb2_Webbutveckling.Models
{
    public class ProductUpdateDto
    {
        public int ProductNumber { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Category { get; set; }
        public bool IsDiscontinued { get; set; }
        public static ProductUpdateDto FromProduct(Product p) => new()
        {
            ProductNumber = p.ProductNumber,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Category = p.Category,
            IsDiscontinued = p.IsDiscontinued
        };
    }
}
