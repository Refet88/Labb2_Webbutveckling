using Blazor_Labb2_Webbutveckling.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Product
{
    [Range(1, int.MaxValue, ErrorMessage = "Product number must be greater than 0.")]
    public int ProductNumber { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int StockQuantity { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    public string? Category { get; set; }

    [JsonPropertyName("isDiscontinued")]
    public bool IsDiscontinued { get; set; } = false;

    public List<ProductImage> Images { get; set; } = new();
}