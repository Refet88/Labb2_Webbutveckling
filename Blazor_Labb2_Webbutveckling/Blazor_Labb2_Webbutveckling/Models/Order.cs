
using System.ComponentModel.DataAnnotations;

namespace Blazor_Labb2_Webbutveckling.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public List<OrderDetails> OrderItems { get; set; } = new List<OrderDetails>();

        public Customer? Customer { get; set; }
    }
}