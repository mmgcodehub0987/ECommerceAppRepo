using ECOMapplication.DTO;
using ECOMapplication.DTOs.RefundDTOs;
using ECOMapplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOMapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly RefundService _RefundService;

        public RefundController(RefundService refundService)
        {
            _RefundService = refundService;
        }
        [HttpGet("GetEligibleRefunds")]
        public async Task<ActionResult<ApiResponse<List<PendingRefundResponseDTO>>>> GetEligibleRefunds()
        {
            var response = await _RefundService.GetEligibleRefundsAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        // POST: api/Refunds/ProcessRefund
        // Initiates a refund for approved cancellations without an existing refund record.
        [HttpPost("ProcessRefund")]
        public async Task<ActionResult<ApiResponse<RefundResponseDTO>>> ProcessRefund([FromBody] RefundRequestDTO refundRequest)
        {
            var response = await _RefundService.ProcessRefundAsync(refundRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        // PUT: api/Refunds/UpdateRefundStatus
        // Manually reprocesses a refund (only applicable if the refund is pending or failed).
        [HttpPut("UpdateRefundStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateRefundStatus([FromBody] RefundStatusUpdateDTO statusUpdate)
        {
            var response = await _RefundService.UpdateRefundStatusAsync(statusUpdate);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        // GET: api/Refunds/GetRefundById/{id}
        // Retrieves a refund by its ID.
        [HttpGet("GetRefundById/{id}")]
        public async Task<ActionResult<ApiResponse<RefundResponseDTO>>> GetRefundById(int id)
        {
            var response = await _RefundService.GetRefundByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        // GET: api/Refunds/GetAllRefunds
        // Retrieves all refunds.
        [HttpGet("GetAllRefunds")]
        public async Task<ActionResult<ApiResponse<List<RefundResponseDTO>>>> GetAllRefunds()
        {
            var response = await _RefundService.GetAllRefundsAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

    }
}
