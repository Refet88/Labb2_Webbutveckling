using Labb2_Webbutveckling.Models;

namespace Labb2_Webbutveckling.Data
{
    public interface IImageRepository
    {
        Task<ProductImage?> GetImageByIdAsync(int id);
        Task<IEnumerable<ProductImage>> GetImagesByProductNumberAsync(int productNumber);
        Task AddImageAsync(ProductImage image);
        Task DeleteImageAsync(int id);
        Task SetPrimaryImageAsync(int productNumber, int imageId);
    }
}
