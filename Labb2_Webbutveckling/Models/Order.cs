using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Labb2_Webbutveckling.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now.Date;

        public Customer? Customer { get; set; }

     
        public List<OrderDetails> OrderItems { get; set; } = new List<OrderDetails>();
    }
}