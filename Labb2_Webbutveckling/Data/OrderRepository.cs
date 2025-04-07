using Labb2_Webbutveckling.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labb2_Webbutveckling.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ECommerceDbContext _context;

        public OrderRepository(ECommerceDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(od => od.Product)
                .ToListAsync();
        }
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }
        public async Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(od => od.Product)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> SearchOrdersAsync(string query)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o =>
                    o.Customer.FirstName.Contains(query) ||
                    o.Customer.LastName.Contains(query) ||
                    o.Customer.Email.Contains(query)
                )
                .ToListAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            if (order == null || order.OrderItems == null || !order.OrderItems.Any())
            {
                throw new ArgumentException("Order must contain at least one product.");
            }

            foreach (var details in order.OrderItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductNumber == details.ProductNumber);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ProductNumber {details.ProductNumber} not found.");
                }

                if (details.Quantity <= 0)
                {
                    throw new InvalidOperationException($"Invalid quantity for product {details.ProductNumber}. Quantity must be greater than 0.");
                }

                if (product.IsDiscontinued)
                {
                    throw new InvalidOperationException($"Product {product.Name} ({product.ProductNumber}) is discontinued and cannot be ordered.");
                }


                if (product.StockQuantity < details.Quantity)
                {
                    throw new InvalidOperationException($"Not enough stock for product {product.Name} ({product.ProductNumber}). Available: {product.StockQuantity}, Requested: {details.Quantity}");
                }

                product.StockQuantity -= details.Quantity;
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return false;
            }

            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductNumber == item.ProductNumber);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}