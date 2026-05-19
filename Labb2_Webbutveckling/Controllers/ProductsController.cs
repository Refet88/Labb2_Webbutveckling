using Labb2_Webbutveckling.Data;
using Labb2_Webbutveckling.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labb2_Webbutveckling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5 MB

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        /// <summary>
        /// Retrieves a list of all products available in the system.
        /// </summary>
        /// <returns>
        /// A response with status code 200 (OK) containing the list of products.  
        /// A response with status code 204 (No Content) if no products are available.
        /// </returns>
        [HttpGet(Name = "GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _unitOfWork.ProductRepository.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Retrieves a specific product by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product to retrieve.</param>
        /// <response code="200">Returns the product object if found.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpGet("{id}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);
        }

        /// <summary>
        /// Searches for products in the system based on a query string.
        /// </summary>
        /// <param name="query">The query string used to search for matching products.</param>
        /// <response code="200">Returns a list of products that match the search query.</response>
        /// <response code="404">If no matching products are found.</response>
        [HttpGet("search/{query}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchProducts(string query)
        {
            var products = await _unitOfWork.ProductRepository.SearchProductsAsync(query);

            if (products == null || !products.Any())
            {

                return NotFound($"No products found for query: {query}");
            }

            return Ok(products);
        }

        /// <summary>
        /// Adds a new product to the system.
        /// </summary>
        /// <param name="product">The product object containing details of the product to be added.</param>
        /// <response code="200">The product was successfully added.</response>
        /// <response code="400">If the product data is null or invalid.</response>
        [HttpPost(Name = "AddProduct")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            await _unitOfWork.ProductRepository.AddProductAsync(product);
            return Ok("Product added successfully.");
        }

        /// <summary>
        /// Updates the details of a specific product by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="product">The updated product object containing the new details.</param>
        /// <response code="200">The product was successfully updated.</response>
        /// <response code="400">If the product data is null or invalid.</response>
        /// <response code="404">If the product with the specified ID was not found.</response>
        [HttpPut("{id}", Name = "UpdateProduct")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (product == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid product data.");
            }

            
            var existingProduct = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            
            var updated = await _unitOfWork.ProductRepository.UpdateProductAsync(id, product);
            if (!updated)
            {
                return NotFound("Product could not be updated.");
            }

            return Ok("Product updated successfully.");
        }


        /// <summary>
        /// Marks a specific product as discontinued by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product to discontinue.</param>
        /// <response code="200">The product was successfully marked as discontinued.</response>
        /// <response code="400">If the product is already marked as discontinued.</response>
        /// <response code="404">If the product with the specified ID was not found.</response>
        [HttpPut("discontinue/{id}", Name = "DiscontinueProduct")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DiscontinueProduct(int id)
        {
            
            var product = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            if (product.IsDiscontinued)
            {
                return BadRequest("The product is already marked as discontinued.");
            }

            
            await _unitOfWork.ProductRepository.DiscontinueProductAsync(id);
            return Ok("The product has been marked as discontinued.");
        }



        /// <summary>
        /// Reactivates a product that has been marked as discontinued.
        /// </summary>
        /// <param name="id">The unique identifier of the product to reactivate.</param>
        /// <response code="200">The product was successfully reactivated.</response>
        /// <response code="400">If the product is already active.</response>
        /// <response code="404">If the product with the specified ID was not found.</response>
        [HttpPut("Reactivate/{id}", Name = "ReactivateProduct")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReactivateProduct(int id)
        {
            
            var product = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            if (!product.IsDiscontinued)
            {
                return BadRequest("The product is already active.");
            }

            
            await _unitOfWork.ProductRepository.ReactivateProductAsync(id);
            return Ok("The product has been reactivated.");
        }



        /// <summary>
        /// Deletes a specific product from the system by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <response code="200">The product was successfully deleted.</response>
        /// <response code="404">If the product with the specified ID was not found.</response>
        [HttpDelete("{id}", Name = "DeleteProduct")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            
            var product = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            
            await _unitOfWork.ProductRepository.DeleteProductAsync(id);
            return Ok("Product deleted successfully.");
        }


        /// <summary>
        /// Updates the stock quantity for a specific product by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update the stock for.</param>
        /// <param name="quantity">The quantity to add to the existing stock (can be negative to reduce stock).</param>
        /// <response code="204">The stock was successfully updated.</response>
        /// <response code="400">If the resulting stock quantity is negative.</response>
        /// <response code="404">If the product with the specified ID was not found.</response>
        [HttpPut("{id}/update-stock", Name = "UpdateStock")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStock(int id, int quantity)
        {
            
            var product = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            
            product.StockQuantity += quantity;

            
            if (product.StockQuantity < 0)
            {
                return BadRequest("Stock cannot be negative.");
            }

            
            await _unitOfWork.ProductRepository.UpdateProductAsync(id, product);

            return Ok(product);
        }

        /// <summary>
        /// Retrieves all images for a product by its unique product number.
        /// </summary>
        /// <param name="id">The unique identifier (product number) of the product whose images to retrieve.</param>
        /// <response code="200">Returns the list of product images ordered by sort order.</response>
        /// <response code="404">If the product was not found.</response>
        [HttpGet("{id}/images", Name = "GetProductImages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductImages(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            var images = await _unitOfWork.ImageRepository.GetImagesByProductNumberAsync(id);
            var ordered = images.OrderBy(i => i.SortOrder).ToList();
            return Ok(ordered);
        }

        /// <summary>
        /// Uploads an image for a product. Uses multipart/form-data with form field name <c>file</c>.
        /// </summary>
        /// <param name="id">The unique identifier (product number) of the product to attach the image to.</param>
        /// <param name="file">The image file to upload.</param>
        /// <response code="201">The image was successfully uploaded and saved.</response>
        /// <response code="400">If no file was uploaded, the file is too large, or the file type is not allowed.</response>
        /// <response code="404">If the product was not found.</response>
        [HttpPost("{id}/images", Name = "UploadProductImage")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadProductImage(int id, IFormFile file)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.Length > MaxImageSizeBytes)
                return BadRequest("File is too large (max 5 MB).");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension))
                return BadRequest("Invalid file type. Allowed: jpg, jpeg, png, webp.");

            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "products", id.ToString());
            Directory.CreateDirectory(uploadDir);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var physicalPath = Path.Combine(uploadDir, fileName);

            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var existingImages = (await _unitOfWork.ImageRepository.GetImagesByProductNumberAsync(id)).ToList();
            var isFirstImage = existingImages.Count == 0;

            var productImage = new ProductImage
            {
                ProductNumber = id,
                ImageUrl = $"/uploads/products/{id}/{fileName}",
                IsPrimary = isFirstImage,
                SortOrder = existingImages.Count
            };

            await _unitOfWork.ImageRepository.AddImageAsync(productImage);

            return CreatedAtAction(
                nameof(GetProductImages),
                new { id },
                productImage);
        }

        /// <summary>
        /// Deletes a product image by its unique image ID (removes the database record and the file from disk).
        /// </summary>
        /// <param name="id">The unique identifier (product number) of the product that owns the image.</param>
        /// <param name="imageId">The unique identifier of the image to delete.</param>
        /// <response code="204">The image was successfully deleted.</response>
        /// <response code="404">If the product or image was not found, or the image does not belong to the product.</response>
        [HttpDelete("{id}/images/{imageId}", Name = "DeleteProductImage")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProductImage(int id, int imageId)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByProductNumberAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            var image = await _unitOfWork.ImageRepository.GetImageByIdAsync(imageId);
            if (image == null || image.ProductNumber != id)
                return NotFound("Image not found.");

            var relativePath = image.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.Combine(_env.WebRootPath, relativePath);
            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);

            var wasPrimary = image.IsPrimary;
            await _unitOfWork.ImageRepository.DeleteImageAsync(imageId);

            if (wasPrimary)
            {
                var remaining = (await _unitOfWork.ImageRepository.GetImagesByProductNumberAsync(id))
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                if (remaining.Count > 0)
                    await _unitOfWork.ImageRepository.SetPrimaryImageAsync(id, remaining[0].Id);
            }

            return NoContent();
        }

    }
}
