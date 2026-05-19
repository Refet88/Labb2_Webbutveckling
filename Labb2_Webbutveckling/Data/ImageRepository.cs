using Labb2_Webbutveckling.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb2_Webbutveckling.Data
{
    public class ImageRepository : IImageRepository
    {
        private readonly ECommerceDbContext _context;

        public ImageRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<ProductImage?> GetImageByIdAsync(int id)
        {
            return await _context.ProductImages.FindAsync(id);
        }

        public async Task AddImageAsync(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int id)
        {
            var image = await _context.ProductImages.FindAsync(id);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetPrimaryImageAsync(int productNumber, int imageId)
        {
            var images = await _context.ProductImages
                .Where(img => img.ProductNumber == productNumber)
                .ToListAsync();

            foreach (var img in images)
            {
                img.IsPrimary = (img.Id == imageId);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductImage>> GetImagesByProductNumberAsync(int productNumber)
        {
            return await _context.ProductImages
                .Where(img => img.ProductNumber == productNumber)
                .ToListAsync();
        }
    }
}
