using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Labb2_Webbutveckling.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;


        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Role { get; set; } = "Customer";

        [JsonIgnore]
        public List<Order> Orders { get; set; } = new List<Order>();

    }

}
