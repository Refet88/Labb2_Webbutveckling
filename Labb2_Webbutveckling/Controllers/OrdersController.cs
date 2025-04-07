using Blazor_Labb2_Webbutveckling.Models;
using Labb2_Webbutveckling.Data;
using Labb2_Webbutveckling.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labb2_Webbutveckling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves a list of all orders from the system.
        /// </summary>
        /// <response code="200">Returns the list of all orders.</response>
        /// <response code="204">If no orders exist in the system.</response>
        [HttpGet(Name = "GetAllOrders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Authorization header received: {authHeader}");

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var orders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();
            return Ok(orders);
        }



        /// <summary>
        /// Retrieves a specific order by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the order to retrieve.</param>
        /// <response code="200">Returns the order object if found.</response>
        /// <response code="404">If the order is not found.</response>
        [HttpGet("{id}", Name = "GetOrderById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "customerId")?.Value;
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            Console.WriteLine($"Role Claim: {roleClaim}");
            Console.WriteLine($"CustomerId Claim: {userIdClaim}");
            Console.WriteLine($"Requested Order ID: {id}");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Invalid or missing customer ID claim.");
            }

            if (string.IsNullOrEmpty(roleClaim))
            {
                return Unauthorized("Invalid or missing role claim.");
            }

            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            if (roleClaim == "Admin" || (roleClaim == "Customer" && order.CustomerId == int.Parse(userIdClaim)))
            {
                return Ok(order);
            }

            return Forbid("You are not authorized to access this order.");
        }

        /// <summary>
        /// Retrieves all orders placed by a specific customer.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer whose orders are being retrieved.</param>
        /// <response code="200">Returns a list of orders associated with the customer.</response>
        /// <response code="404">If the customer has no orders or does not exist.</response>
        [HttpGet("customer/{customerId}", Name = "GetOrdersByCustomerId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrdersByCustomerId(int customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound($"Customer with ID {customerId} does not exist.");
            }

            var orders = await _unitOfWork.OrderRepository.GetOrdersByCustomerIdAsync(customerId);

            if (orders == null || orders.Count == 0)
            {
                return NotFound($"No orders found for customer with ID {customerId}.");
            }

            return Ok(orders);
        }



        /// <summary>
        /// Searches for orders based on the customer's name, lastname or email.
        /// </summary>
        /// <param name="query">The search string is used to match the customer's name, lastname or email.</param>
        /// <response code="200">Returns a list of orders matching the search query.</response>
        /// <response code="400">If the search query is null or empty.</response>
        /// <response code="404">If no orders match the search query.</response>
        [HttpGet("search/{query}", Name = "SearchOrders")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchOrdersByCustomer(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be null or empty.");
            }

            var orders = await _unitOfWork.OrderRepository.SearchOrdersAsync(query);

            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for customer with query '{query}'.");
            }

            return Ok(orders);
        }

        /// <summary>
        /// Creates a new order for an existing customer.
        /// </summary>
        /// <param name="orderRequest">The order request object containing the customer ID and order details.</param>
        /// <response code="201">Returns the created order if successfully created.</response>
        /// <response code="400">If the order request data is invalid.</response>
        /// <response code="404">If the specified customer is not found.</response>
        [HttpPost("create", Name = "AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequest orderRequest)
        {
            Console.WriteLine($"Received API Call for AddOrder");
            Console.WriteLine($"CustomerId: {orderRequest.CustomerId}");
            foreach (var item in orderRequest.OrderItems)
            {
                Console.WriteLine($"ProductNumber: {item.ProductNumber}, Quantity: {item.Quantity}");
            }

            var existingCustomer = await _unitOfWork.CustomerRepository.GetCustomerByIdAsync(orderRequest.CustomerId);
            if (existingCustomer == null)
            {
                return NotFound("Customer not found.");
            }

            var order = new Order
            {
                CustomerId = orderRequest.CustomerId,
                OrderItems = orderRequest.OrderItems.Select(oi => new OrderDetails
                {
                    ProductNumber = oi.ProductNumber,
                    Quantity = oi.Quantity
                }).ToList()
            };

            foreach (var details in order.OrderItems)
            {
                var product = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(details.ProductNumber);
                if (product == null)
                {
                    return BadRequest($"Product {details.ProductNumber} not found.");
                }

                details.Price = product.Price;
            }

            try
            {
                await _unitOfWork.OrderRepository.AddOrderAsync(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a specific order by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the order to delete.</param>
        /// <response code="200">The order was successfully deleted.</response>
        /// <response code="404">If the order with the specified ID was not found.</response>
        [HttpDelete("{id}", Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var success = await _unitOfWork.OrderRepository.DeleteOrderAsync(id);

            if (!success)
            {
                return NotFound("Order not found.");
            }

            return Ok("Order deleted successfully.");
        }
    }
}