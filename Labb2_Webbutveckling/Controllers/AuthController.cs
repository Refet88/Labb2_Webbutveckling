using Labb2_Webbutveckling.Data;
using Labb2_Webbutveckling.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Retrieves a token if login is successfull.
    /// </summary>
    /// <param name="loginRequest">The login credentials containing the username and password.</param>
    /// <response>
    /// An HTTP 200 OK response with a JWT token if authentication is successful.
    /// An HTTP 400 Bad Request if the username or password is null or empty.
    /// An HTTP 401 Unauthorized if the user is not found or the credentials are invalid.
    /// </response>
    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> Login([FromBody] Login loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            return BadRequest("Username or password cannot be null or empty.");
        }

        if (loginRequest.Username.Contains("@"))
        {
            var customer = await _unitOfWork.CustomerRepository.GetCustomerByEmailAsync(loginRequest.Username);
            if (customer == null) return Unauthorized("Customer not found.");

            var passwordHasher = new PasswordHasher<Customer>();
            var result = passwordHasher.VerifyHashedPassword(customer, customer.Password, loginRequest.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = GenerateJwtToken(customer.Email, "Customer", customer.CustomerId.ToString());
            return Ok(new { Token = token });
        }
        else
        {
            var admin = await _unitOfWork.AdminRepository.GetAdminByUsernameAsync(loginRequest.Username);
            if (admin == null) return Unauthorized("Admin not found.");

            var passwordHasher = new PasswordHasher<Admin>();
            var result = passwordHasher.VerifyHashedPassword(admin, admin.Password, loginRequest.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = GenerateJwtToken(admin.Username, "Admin", admin.AdminId.ToString());
            return Ok(new { Token = token });
        }
    }

    private string GenerateJwtToken(string username, string role, string customerId)
    {
        var claims = new[]
        {
            new Claim("customerId", customerId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };
        foreach (var claim in claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}