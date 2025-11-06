using ECOMapplication.DBContext;
using ECOMapplication.DTO;
using ECOMapplication.DTOs.FeedBackDTOs;
using ECOMapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ECOMapplication.Services
{
    public class FeedBackService
    {
        private readonly ApplicationDbContext _DbContext;
        public FeedBackService(ApplicationDbContext context)
        {
            _DbContext = context;
        }
        public async Task<ApiResponse<FeedBackResponseDTO>> SubmitFeedbackAsync(FeedBackCreateDTO feedbackCreateDTO)
        {
            try
            {
                // Verify customer exists
                var customer = await _DbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == feedbackCreateDTO.CustomerId);
                if (customer == null)
                {
                    return new ApiResponse<FeedBackResponseDTO>(404, "Customer not found.");
                }
                // Verify product exists
                var product = await _DbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == feedbackCreateDTO.ProductId);
                if (product == null)
                {
                    return new ApiResponse<FeedBackResponseDTO>(404, "Product not found.");
                }

                // Verify order item exists and belongs to customer and product (Order must be delivered)
                var orderItem = await _DbContext.OrdersItems
                .Include(oi => oi.Order)
                .AsNoTracking()
                .FirstOrDefaultAsync(oi =>oi.ProdctId == feedbackCreateDTO.ProductId &&
                oi.Order.CustomerId == feedbackCreateDTO.CustomerId &&
                oi.Order.OrderStatus == OrderStatus.Delivered);
                if (orderItem == null)
                {
                    return new ApiResponse<FeedBackResponseDTO>(400, "Invalid OrderItemId. Customer must have purchased the product.");
                }
                // Check if feedback already exists for this order item
                if (await _DbContext.Feedbacks.AnyAsync(fed => fed.CustomerId == feedbackCreateDTO.CustomerId && fed.ProductId == feedbackCreateDTO.ProductId))
                {
                    return new ApiResponse<FeedBackResponseDTO>(400, "Feedback for this product and order item already exists.");
                }
                // Create new feedback entity
                var feedback = new Feedback
                {
                    CustomerId = feedbackCreateDTO.CustomerId,
                    ProductId = feedbackCreateDTO.ProductId,
                    Rating = feedbackCreateDTO.Rating,
                    Comment = feedbackCreateDTO.Comment,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _DbContext.Feedbacks.Add(feedback);
                await _DbContext.SaveChangesAsync();
                // Prepare response DTO (manual mapping)
                var feedbackResponse = new FeedBackResponseDTO
                {
                    Id = feedback.Id,
                    CustomerId = customer.Id,
                    CustomerName = $"{customer.FirstName} {customer.LastName}",
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    Rating = feedback.Rating,
                    Comment = feedback.Comment,
                    CreatedAt = feedback.CreatedAt,
                    UpdatedAt = feedback.UpdatedAt
                };
                return new ApiResponse<FeedBackResponseDTO>(200, feedbackResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (implementation depends on your logging setup)
                return new ApiResponse<FeedBackResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
        // Retrieves all feedback for a specific product along with the average rating.
        public async Task<ApiResponse<ProductFeedbackResponseDTO>> GetFeedbackForProductAsync(int productId)
        {
            try
            {
                // Verify product exists
                var product = await _DbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    return new ApiResponse<ProductFeedbackResponseDTO>(404, "Product not found.");
                }
                // Retrieve feedbacks for the specified product, including customer details, with no tracking for performance
                var feedbacks = await _DbContext.Feedbacks
                .Where(f => f.ProductId == productId)
                .Include(f => f.Customer)
                .AsNoTracking()
                .ToListAsync();
                double averageRating = 0;
                List<CustomerFeedbackDTO> customerFeedbacks = new List<CustomerFeedbackDTO>();
                if (feedbacks.Any())
                {
                    averageRating = feedbacks.Average(f => f.Rating);
                    customerFeedbacks = feedbacks.Select(f => new CustomerFeedbackDTO
                    {
                        Id = f.Id,
                        CustomerId = f.CustomerId,
                        CustomerName = $"{f.Customer.FirstName} {f.Customer.LastName}",
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CreatedAt = f.CreatedAt,
                        UpdatedAt = f.UpdatedAt
                    }).ToList();
                }
                var productFeedbackResponse = new ProductFeedbackResponseDTO
                {
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    AverageRating = Math.Round(averageRating, 2),
                    Feedbacks = customerFeedbacks
                };
                return new ApiResponse<ProductFeedbackResponseDTO>(200, productFeedbackResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (implementation depends on your logging setup)
                return new ApiResponse<ProductFeedbackResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
        // Retrieves all feedback entries in the system.
        public async Task<ApiResponse<List<FeedBackResponseDTO>>> GetAllFeedbackAsync()
        {
            try
            {
                var feedbacks = await _DbContext.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Product)
                .AsNoTracking()
                .ToListAsync();
                var feedbackResponseList = feedbacks.Select(f => new FeedBackResponseDTO
                {
                    Id = f.Id,
                    CustomerId = f.CustomerId,
                    CustomerName = $"{f.Customer.FirstName} {f.Customer.LastName}",
                    ProductId = f.ProductId,
                    ProductName = f.Product.ProductName,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList();
                return new ApiResponse<List<FeedBackResponseDTO>>(200, feedbackResponseList);
            }
            catch (Exception ex)
            {
                // Log the exception (implementation depends on your logging setup)
                return new ApiResponse<List<FeedBackResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
        // Updates an existing feedback entry.
        public async Task<ApiResponse<FeedBackResponseDTO>> UpdateFeedbackAsync(FeedBackUpdateDTO feedbackUpdateDTO)
        {
            try
            {
                // Retrieve the feedback along with its customer and product information
                var feedback = await _DbContext.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Product)
                .FirstOrDefaultAsync(f => f.Id == feedbackUpdateDTO.FeedbackId
                && f.CustomerId == feedbackUpdateDTO.CustomerId);
                if (feedback == null)
                {
                    return new ApiResponse<FeedBackResponseDTO>(404, "Either Feedback or Customer not found.");
                }
                // Update the feedback details
                feedback.Rating = feedbackUpdateDTO.Rating;
                feedback.Comment = feedbackUpdateDTO.Comment;
                feedback.UpdatedAt = DateTime.UtcNow;
                await _DbContext.SaveChangesAsync();
                var feedbackResponse = new FeedBackResponseDTO
                {
                    Id = feedback.Id,
                    CustomerId = feedback.CustomerId,
                    CustomerName = $"{feedback.Customer.FirstName} {feedback.Customer.LastName}",
                    ProductId = feedback.ProductId,
                    ProductName = feedback.Product.ProductName,
                    Rating = feedback.Rating,
                    Comment = feedback.Comment,
                    CreatedAt = feedback.CreatedAt,
                    UpdatedAt = feedback.UpdatedAt
                };
                return new ApiResponse<FeedBackResponseDTO>(200, feedbackResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (implementation depends on your logging setup)
                return new ApiResponse<FeedBackResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
        // Deletes a feedback entry.
        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteFeedbackAsync(FeedBackDeleteDTO feedbackDeleteDTO)
        {
            try
            {
                var feedback = await _DbContext.Feedbacks.FindAsync(feedbackDeleteDTO.FeedbackId);
                if (feedback == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Feedback not found.");
                }
                // Ensure that only the owner can delete the feedback
                if (feedback.CustomerId != feedbackDeleteDTO.CustomerId)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(401, "You are not authorized to delete this feedback.");
                }
                _DbContext.Feedbacks.Remove(feedback);
                await _DbContext.SaveChangesAsync();
                var confirmation = new ConfirmationResponseDTO
                {
                    Message = $"Feedback with Id {feedbackDeleteDTO.FeedbackId} deleted successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception ex)
            {
                // Log the exception (implementation depends on your logging setup)
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
    }
}
