using ECOMapplication.DBContext;
using ECOMapplication.DTO;
using ECOMapplication.DTOs.ProductDTOs;
using ECOMapplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ECOMapplication.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _DbContext;

        public ProductService(ApplicationDbContext context)
        {
            _DbContext = context;
        }

        public async Task<ApiResponse<ProductResponseDTO>> CreateProduct(ProductCreateDTO productCreateDTO)
        {
            try
            {
                if (await _DbContext.Products.AnyAsync(p => p.ProductName.ToLower() == productCreateDTO.Name.ToLower()))
                    return new ApiResponse<ProductResponseDTO>(401, "A product with the same name already exists.");

                if (!await _DbContext.Categories.AnyAsync(cat => cat.Id == productCreateDTO.CategoryId))
                    return new ApiResponse<ProductResponseDTO>(401, "Specified category does not exists");

                var product = new Product()
                {
                    ProductName = productCreateDTO.Name,
                    Description = productCreateDTO.Description,
                    Price = productCreateDTO.Price,
                    StockQuantity = productCreateDTO.StockQuantity,
                    ImageURL = productCreateDTO.ImageUrl,
                    DiscountPercentage = productCreateDTO.DiscountPercentage,
                    IsAvailable = true,
                    CategoryId = productCreateDTO.CategoryId
                };

                _DbContext.Products.Add(product);
                await _DbContext.SaveChangesAsync();

                //create response
                var createResponse = new ProductResponseDTO()
                {
                    Id = product.Id,
                    Name = product.ProductName,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageURL,
                    DiscountPercentage = product.DiscountPercentage,
                    CategoryId = product.CategoryId,
                    IsAvailable = product.IsAvailable
                };
                return new ApiResponse<ProductResponseDTO>(200, createResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductResponseDTO>> GetProductById(int ProductId)
        {
            try
            {
                var product = await _DbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == ProductId);
                if (product == null)
                    return new ApiResponse<ProductResponseDTO>(404, "Product Not found !");

                var response = new ProductResponseDTO()
                {
                    Id = product.Id,
                    Name = product.ProductName,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageURL,
                    DiscountPercentage = product.DiscountPercentage,
                    CategoryId = product.CategoryId,
                    IsAvailable = product.IsAvailable
                };
                return new ApiResponse<ProductResponseDTO>(200, response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ProductResponseDTO>>> GetProductListByCategoryID(int CategoryId)
        {
            try
            {
                var products = await _DbContext.Products.AsNoTracking().Include(p => p.Category)
                    .Where(p => p.CategoryId == CategoryId && p.IsAvailable)
                    .ToListAsync();
                if (products.Count == 0 && products == null)
                    return new ApiResponse<List<ProductResponseDTO>>(404, "Product Not found under the specified category");

                var productListResponse = products.Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageURL,
                    DiscountPercentage = p.DiscountPercentage,
                    CategoryId = p.CategoryId,
                    IsAvailable = p.IsAvailable
                }).ToList();

                return new ApiResponse<List<ProductResponseDTO>>(200, productListResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }

        }

        public async Task<ApiResponse<List<ProductResponseDTO>>> GetAllProducts()
        {
            try
            {
                var products = await _DbContext.Products.AsNoTracking().ToListAsync();
                if (products.Count == 0 || products == null)
                {
                    return new ApiResponse<List<ProductResponseDTO>>(404, "No products found");
                }

                var productList = products.Select(p => new ProductResponseDTO
                {

                    Id = p.Id,
                    Name = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageURL,
                    DiscountPercentage = p.DiscountPercentage,
                    CategoryId = p.CategoryId,
                    IsAvailable = p.IsAvailable
                }
                ).ToList();
                return new ApiResponse<List<ProductResponseDTO>>(200, productList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateProductStatus(ProductStatusUpdateDTO productStatusUpdateDTO)
        {
            try
            {
                var product = await _DbContext.Products.FirstOrDefaultAsync(p => p.Id == productStatusUpdateDTO.ProductId);
                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");
                }
                product.IsAvailable = productStatusUpdateDTO.IsAvailable;
                await _DbContext.SaveChangesAsync();


                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Product with Id '{product.ProductName}' Status Updated successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateProduct(ProductUpdateDTO productDto)
        {
            try
            {
                var product = await _DbContext.Products.FindAsync(productDto.Id);
                if (product == null)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");

                if (await _DbContext.Products.AnyAsync(p => p.ProductName.ToLower() == productDto.Name.ToLower() && p.Id != productDto.Id))
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Another product with the same name already exists.");

                if (!await _DbContext.Categories.AnyAsync(cat => cat.Id == productDto.CategoryId))
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Specified category does not exist.");

                product.ProductName = productDto.Name;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.StockQuantity = productDto.StockQuantity;
                product.ImageURL = productDto.ImageUrl;
                product.DiscountPercentage = productDto.DiscountPercentage;
                product.CategoryId = productDto.CategoryId;

                await _DbContext.SaveChangesAsync();

                // Prepare confirmation message
                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Product '{productDto.Name}' updated successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }

        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _DbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");

                //Soft Delete
                product.IsAvailable = false;
                await _DbContext.SaveChangesAsync();
                // Prepare confirmation message
                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Product '{product.ProductName}' deleted successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
    }
}
