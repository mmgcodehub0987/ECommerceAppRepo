using ECOMapplication.DTO;
using ECOMapplication.DTOs.PaymentDTOs;
using ECOMapplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOMapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost("ProcessPayment")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDTO>>> ProcessPayment([FromBody] PaymentRequestDTO paymentRequest)
        {
            var response = await _paymentService.ProcessPaymentAsync(paymentRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves payment details by Payment ID.
        [HttpGet("GetPaymentById/{paymentId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDTO>>> GetPaymentById(int paymentId)
        {
            var response = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves payment details associated with a specific order.
        [HttpGet("GetPaymentByOrderId/{orderId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDTO>>> GetPaymentByOrderId(int orderId)
        {
            var response = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates the status of an existing payment.
        [HttpPut("UpdatePaymentStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdatePaymentStatus([FromBody] PaymentStatusUpdateDTO statusUpdate)
        {
            var response = await _paymentService.UpdatePaymentStatusAsync(statusUpdate);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Completes a Cash on Delivery (COD) payment.
        [HttpPost("CompleteCODPayment")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> CompleteCODPayment([FromBody] CODPaymentUpdateDTO codPaymentUpdateDTO)
        {
            var response = await _paymentService.CompleteCODPaymentAsync(codPaymentUpdateDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}