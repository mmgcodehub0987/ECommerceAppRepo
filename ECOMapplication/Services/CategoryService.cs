using ECOMapplication.DBContext;
using ECOMapplication.DTOs.CategoryDTOs;
using ECOMapplication.DTOs;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ECOMapplication.DTO;
using Microsoft.EntityFrameworkCore;
using ECOMapplication.Models;

namespace ECOMapplication.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _DbContext;

        public CategoryService(ApplicationDbContext context)
        {
            _DbContext = context;
        }

        public async Task<ApiResponse<CategoryResponseDTO>> CreateCategory(CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
                if(await _DbContext.Categories.AnyAsync( cat => cat.CategoryName.ToLower() == categoryCreateDTO.Name.ToLower()))
                {
                    return new ApiResponse<CategoryResponseDTO>(400, "Category already exists.");
                }

                //create a category
                var category = new Category()
                {
                    CategoryName = categoryCreateDTO.Name,
                    CategoryDescription = categoryCreateDTO.Description,
                    IsActive = true
                };

                _DbContext.Categories.Add(category);
                await _DbContext.SaveChangesAsync();

                //response
                var categoryResponse = new CategoryResponseDTO()
                {
                    Id = category.Id,
                    Name = category.CategoryName,
                    Description = category.CategoryDescription,
                    IsActive = true
                };
                return new ApiResponse<CategoryResponseDTO>(200, categoryResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CategoryResponseDTO>> GetCategorybyId(int categoryId)
        {
            try
            {
                var category = await _DbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c=> c.Id == categoryId);
                if (category == null)
                    return new ApiResponse<CategoryResponseDTO>(404, "category not found");

                var categoryResponse = new CategoryResponseDTO()
                {
                    Id = category.Id,
                    Name = category.CategoryName,
                    Description = category.CategoryDescription,
                    IsActive = true
                };
                return new ApiResponse<CategoryResponseDTO> (200, categoryResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateCategory (CategoryUpdateDTO categoryUpdateDTO)
        {
            try
            {
                var category = await _DbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryUpdateDTO.Id);
                if (category == null)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Category not found");
                if (!(string.Equals(category.CategoryName, categoryUpdateDTO.Name, StringComparison.OrdinalIgnoreCase)))
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Category with same name already exists");

                category.CategoryName = categoryUpdateDTO.Name;
                category.CategoryDescription = categoryUpdateDTO.Description;

                await _DbContext.SaveChangesAsync();
                var respondMessage = new ConfirmationResponseDTO()
                {
                    Message = $"Category Name: {category.CategoryName} upadted successfully"
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, respondMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteCategory(int categoryId)
        {
            try
            {
                var category = await _DbContext.Categories.Include(c=> c.Products).FirstOrDefaultAsync(c=> c.Id == categoryId);
                if(category == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "User not found");
                }
                //soft delete
                category.IsActive = false;
                await _DbContext.SaveChangesAsync();

                var respondMessage = new ConfirmationResponseDTO()
                {
                    Message = $"category: {category.CategoryName} deleted successfully"
                };
                return new ApiResponse<ConfirmationResponseDTO> (200, respondMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CategoryResponseDTO>>> GetAllCategories()
        {
            try
            {
                var categories = await _DbContext.Categories.AsNoTracking()
                    .ToListAsync();
                var categoryList = categories.Select(c => new CategoryResponseDTO
                {
                    Id = c.Id,
                    Name = c.CategoryName,
                    Description = c.CategoryDescription,
                    IsActive = c.IsActive
                }).ToList();
                return new ApiResponse<List<CategoryResponseDTO>>(200, categoryList);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse<List<CategoryResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }


    }
}
