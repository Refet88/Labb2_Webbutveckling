using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Labb2_Webbutveckling.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductNumber { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Category { get; set; }

        [JsonPropertyName("isDiscontinued")]
        public bool IsDiscontinued { get; set; } = false;

        [JsonIgnore]
        public List<OrderDetails> OrderItems { get; set; } = new List<OrderDetails>();

        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}