using ECOMapplication.DTO;
using ECOMapplication.DTOs.CancellationDTOs;
using ECOMapplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOMapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationController : ControllerBase
    {
        private readonly CancellationService _cancellationService;

        public CancellationController(CancellationService cancellationService)
        {
            _cancellationService = cancellationService;
        }
        [HttpPost("RequestCancellation")]
        public async Task<ActionResult<ApiResponse<CancellationResponseDTO>>> RequestCancellation([FromBody] CancellationRequestDTO cancellationRequest)
        {
            var response = await _cancellationService.RequestCancellationAsync(cancellationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        // Endpoint to retrieve all cancellation requests.
        [HttpGet("GetAllCancellations")]
        public async Task<ActionResult<ApiResponse<List<CancellationResponseDTO>>>> GetAllCancellations()
        {
            var response = await _cancellationService.GetAllCancellationsAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        // Endpoint to retrieve cancellation details by cancellation ID.
        [HttpGet("GetCancellationById/{id}")]
        public async Task<ActionResult<ApiResponse<CancellationResponseDTO>>> GetCancellationById(int id)
        {
            var response = await _cancellationService.GetCancellationByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        // Endpoint for administrators to update the status of a cancellation request.
        [HttpPut("UpdateCancellationStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateCancellationStatus([FromBody] CancellationStatusUpdateDTO statusUpdate)
        {
            var response = await _cancellationService.UpdateCancellationStatusAsync(statusUpdate);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
