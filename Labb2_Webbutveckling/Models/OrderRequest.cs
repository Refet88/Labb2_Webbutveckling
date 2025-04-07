namespace Blazor_Labb2_Webbutveckling.Models
{
    public class OrderRequest
    {
        public int CustomerId { get; set; }
        public List<OrderItemRequest> OrderItems { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public int ProductNumber { get; set; }
        public int Quantity { get; set; }
    }
}

