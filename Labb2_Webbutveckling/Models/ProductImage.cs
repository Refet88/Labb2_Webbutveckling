using System.Text.Json.Serialization;

namespace Labb2_Webbutveckling.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public int ProductNumber { get; set; }

        [JsonIgnore]
        public Product? Product { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
    }
}
