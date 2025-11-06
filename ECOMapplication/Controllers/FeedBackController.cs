using ECOMapplication.DTO;
using ECOMapplication.DTOs.FeedBackDTOs;
using ECOMapplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOMapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        private readonly FeedBackService _FeedbackService;
        public FeedBackController(FeedBackService feedbackService)
        {
            _FeedbackService = feedbackService;
        }

        [HttpPost("SubmitFeedback")]
        public async Task<ActionResult<ApiResponse<FeedBackResponseDTO>>> SubmitFeedback([FromBody] FeedBackCreateDTO feedbackCreateDTO)
        {
            var response = await _FeedbackService.SubmitFeedbackAsync(feedbackCreateDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all feedback for a specific product.
        [HttpGet("GetFeedbackForProduct/{productId}")]
        public async Task<ActionResult<ApiResponse<ProductFeedbackResponseDTO>>> GetFeedbackForProduct(int productId)
        {
            var response = await _FeedbackService.GetFeedbackForProductAsync(productId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all feedback (Admin use).
        [HttpGet("GetAllFeedback")]
        public async Task<ActionResult<ApiResponse<List<FeedBackResponseDTO>>>> GetAllFeedback()
        {
            var response = await _FeedbackService.GetAllFeedbackAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates a specific feedback entry.
        [HttpPut("UpdateFeedback")]
        public async Task<ActionResult<ApiResponse<FeedBackResponseDTO>>> UpdateFeedback([FromBody] FeedBackUpdateDTO feedbackUpdateDTO)
        {
            var response = await _FeedbackService.UpdateFeedbackAsync(feedbackUpdateDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Deletes a specific feedback entry.
        [HttpDelete("DeleteFeedback")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> DeleteFeedback([FromBody] FeedBackDeleteDTO feedbackDeleteDTO)
        {
            var response = await _FeedbackService.DeleteFeedbackAsync(feedbackDeleteDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
