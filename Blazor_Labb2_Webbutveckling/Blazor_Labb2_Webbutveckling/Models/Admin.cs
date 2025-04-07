namespace Blazor_Labb2_Webbutveckling.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "Admin";
    }
}