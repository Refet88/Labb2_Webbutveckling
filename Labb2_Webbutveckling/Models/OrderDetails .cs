using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Labb2_Webbutveckling.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }
        public int ProductNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }


        [JsonIgnore]
        public Order? Order { get; set; }
        public Product? Product { get; set; } = null;
    }
}
