using Microsoft.AspNetCore.Mvc;
using Labb2_Webbutveckling.Data;
using Labb2_Webbutveckling.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Labb2_Webbutveckling.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves a list of all the customers.
        /// </summary>
        /// <response code="200">Returns a list of all customers.</response>
        [HttpGet(Name = "GetAllCustomers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllCustomersAsync();

            return Ok(customers);
        }

        /// <summary>
        /// Retrieves a specific customer by their unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the customer to retrieve.</param>
        /// <response code="200">Returns the customer object if found.</response>
        /// <response code="404">If the customer does not exist.</response>
        [HttpGet("{id}", Name = "GetCustomerById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "customerId")?.Value;

            if (userRole == "Customer")
            {
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Customer ID not found in token.");
                }

                if (id != int.Parse(userIdClaim))
                {
                    return Forbid("You are not authorized to access this customer's data.");
                }
            }

            var customer = await _unitOfWork.CustomerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            return Ok(customer);
        }


        /// <summary>
        /// Retrieves a customer by their email address.
        /// </summary>
        /// <param name="email">The email address of the customer to retrieve.</param>
        /// <response code="200">Returns the customer object if found.</response>
        /// <response code="404">If the customer does not exist.</response>
        [HttpGet("email/{email}", Name = "GetCustomerByEmail")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomerByEmail(string email)
        {
            Console.WriteLine($"API received email: {email}");
            var customer = await _unitOfWork.CustomerRepository.GetCustomerByEmailAsync(email);
            Console.WriteLine(customer != null ? $"Customer found: {customer.Email}" : "Customer not found.");
            return customer != null ? Ok(customer) : NotFound();
        }

        /// <summary>
        /// Registers a new customer in the system.
        /// </summary>
        /// <param name="customer">The customer object containing the customer's information to register.</param>
        /// <response code="201">Returns the registered customer object if successfully created.</response>
        /// <response code="400">If the input data is invalid or the email already exists.</response>
        [HttpPost("register", Name = "RegisterCustomer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Customer>> RegisterCustomer([FromBody] Customer customer)
        {
            var existingCustomerByEmail = await _unitOfWork.CustomerRepository.GetCustomerByEmailAsync(customer.Email);
            if (existingCustomerByEmail != null)
            {
                return BadRequest("A customer with this email already exists.");
            }

            customer.Role = "Customer";

            var passwordHasher = new PasswordHasher<Customer>();
            customer.Password = passwordHasher.HashPassword(customer, customer.Password);

            await _unitOfWork.CustomerRepository.AddCustomerAsync(customer);

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
        }


        /// <summary>
        /// Updates the information of an existing customer.
        /// </summary>
        /// <param name="id">The unique identifier of the customer to update.</param>
        /// <param name="updatedCustomer">The updated customer object containing the new data.</param>
        /// <response code="200">The customers details were successfully updated.</response>
        /// <response code="400">If the provided customer data is null or invalid.</response>
        /// <response code="404">If no customer with the specified ID was found.</response>
        [HttpPut("{id}", Name = "UpdateCustomer")]
        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            if (updatedCustomer == null)
            {
                return BadRequest("Request body is missing or invalid.");
            }

            var userRole = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "customerId")?.Value;

            if (userRole == "Customer")
            {
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Customer ID not found in token.");
                }

                if (id != int.Parse(userIdClaim))
                {
                    return Forbid("You are not authorized to update this customer.");
                }
            }

            var updated = await _unitOfWork.CustomerRepository.UpdateCustomerAsync(id, updatedCustomer);

            if (!updated)
            {
                return NotFound("Customer not found.");
            }

            return Ok("Customer updated successfully.");
        }



        /// <summary>
        /// Deletes an existing customer from the system by their unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the customer to delete.</param>
        /// <response code="204">The customer was successfully deleted.</response>
        /// <response code="404">If no customer with the specified ID was found.</response>
        [HttpDelete("{id}", Name = "DeleteCustomer")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }
            await _unitOfWork.CustomerRepository.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}