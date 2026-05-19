using Blazor_Labb2_Webbutveckling.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Product
{
    [Range(1, int.MaxValue, ErrorMessage = "ProductNumber must be greater than zero.")]
    public int ProductNumber { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    [Required]
    public string? Category { get; set; }

    [JsonPropertyName("isDiscontinued")]
    public bool IsDiscontinued { get; set; } = false;

    public List<ProductImage> Images { get; set; } = new();
}