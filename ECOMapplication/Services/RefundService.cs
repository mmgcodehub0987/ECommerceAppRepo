using ECOMapplication.DBContext;
using ECOMapplication.DTO;
using ECOMapplication.DTOs.RefundDTOs;
using ECOMapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ECOMapplication.Services
{
    public class RefundService
    {
        private readonly ApplicationDbContext _DbContext;
        private readonly EmailService _EmailService;

        public RefundService(ApplicationDbContext applicationDbContext, EmailService emailService)
        {
            _DbContext = applicationDbContext;
            _EmailService = emailService;
        }

        public async Task<ApiResponse<List<PendingRefundResponseDTO>>> GetEligibleRefundsAsync()
        {
            var eligible = await _DbContext.Cancellation
            .Include(c => c.Order)
            .ThenInclude(o => o.Payment)
            .Where(c => c.CancellationStatus == CancellationStatus.Approved && c.Refund == null
            && c.Order.Payment.PaymentMethod.ToLower() != "cod")
            .Select(c => new PendingRefundResponseDTO
            {
                CancellationId = c.Id,
                OrderId = c.OrderId,
                OrderAmount = c.OrderAmount,
                CancellationCharge = c.CancellationCharges,
                ComputedRefundAmount = c.OrderAmount - c.CancellationCharges,
                CancellationRemarks = c.Remarks
            }).ToListAsync();
            return new ApiResponse<List<PendingRefundResponseDTO>>(200, eligible);
        }

        public async Task<ApiResponse<RefundResponseDTO>> ProcessRefundAsync(RefundRequestDTO refundRequest)
        {
            // Retrieve the cancellation with related Order, Payment, and Customer details.
            var cancellation = await _DbContext.Cancellation
            .Include(c => c.Order)
            .ThenInclude(o => o.Payment)
            .Include(c => c.Order)
            .ThenInclude(o => o.Customer)
            .FirstOrDefaultAsync(c => c.Id == refundRequest.CancellationId);
            if (cancellation == null)
                return new ApiResponse<RefundResponseDTO>(404, "Cancellation request not found.");

            if (cancellation.CancellationStatus != CancellationStatus.Approved)
                return new ApiResponse<RefundResponseDTO>(400, "Only approved cancellations are eligible for refunds.");

            // Only proceed if no refund record exists.
            var existingRefund = await _DbContext.Refunds
            .FirstOrDefaultAsync(r => r.CancellationId == refundRequest.CancellationId);

            if (existingRefund != null)
                return new ApiResponse<RefundResponseDTO>(400, "Refund for this cancellation request has already been initiated.");

            // Validate that a Payment record exists.
            var payment = cancellation.Order.Payment;
            if (payment == null || payment.PaymentMethod.ToLower() == "cod")
                return new ApiResponse<RefundResponseDTO>(400, "No payment associated with the order.");
            // Compute the refund amount.
            decimal computedRefundAmount = cancellation.OrderAmount - cancellation.CancellationCharges;
            if (computedRefundAmount <= 0)
                return new ApiResponse<RefundResponseDTO>(400, "Computed refund amount is not valid.");
            // Create a new refund record.
            var refund = new Refund
            {
                CancellationId = refundRequest.CancellationId,
                PaymentId = payment.Id,
                RefundAmount = computedRefundAmount,
                RefundMethod = refundRequest.RefundMethod.ToString(),
                RefunndReason = refundRequest.RefundReason,
                RefundStatus = RefundStatus.Pending,
                InitiatedAt = DateTime.UtcNow,
                ProcessedBy = refundRequest.ProcessedBy
            };
            _DbContext.Refunds.Add(refund);
            await _DbContext.SaveChangesAsync();
            // Immediately try processing via the simulated payment gateway.
            var gatewayResponse = await ProcessRefundPaymentAsync(refund);
            if (gatewayResponse.IsSuccess)
            {
                refund.RefundStatus = RefundStatus.Completed;
                refund.TransactionId = gatewayResponse.TransactionId;
                refund.CompletedAt = DateTime.UtcNow;
                payment.PaymentStatus = PaymentStatus.Refunded;
                _DbContext.Payments.Update(payment);
                // Send email notification.
                if (cancellation.Order.Customer != null && !string.IsNullOrEmpty(cancellation.Order.Customer.Email))
                {
                    await _EmailService.SendEmailAsync(
                    cancellation.Order.Customer.Email,
                    $"Your Refund Has Been Processed Successfully, Order #{cancellation.Order.OrderNumber}",
                    GenerateRefundSuccessEmailBody(refund, cancellation.Order.OrderNumber.ToString(), cancellation),
                    IsBodyHtml: true);
                }
            }
            else
            {
                refund.RefundStatus = RefundStatus.Failed;
            }
            _DbContext.Refunds.Update(refund);
            await _DbContext.SaveChangesAsync();
            return new ApiResponse<RefundResponseDTO>(200, MapRefundToDTO(refund));
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateRefundStatusAsync(RefundStatusUpdateDTO statusUpdate)
        {
            var refund = await _DbContext.Refunds
            .Include(r => r.Cancellation)
            .ThenInclude(c => c.Order)
            .ThenInclude(o => o.Customer)
            .Include(r => r.Payment)
            .FirstOrDefaultAsync(r => r.Id == statusUpdate.RefundId);


            if (refund == null)
                return new ApiResponse<ConfirmationResponseDTO>(404, "Refund not found.");

            // Allow manual update only if refund is Pending or Failed.
            if (refund.RefundStatus != RefundStatus.Pending && refund.RefundStatus != RefundStatus.Failed)
                return new ApiResponse<ConfirmationResponseDTO>(400, "Only pending or failed refunds can be updated.");

            // In a manual update, we reprocess the refund.
            refund.RefundMethod = statusUpdate.RefundMethod.ToString();
            refund.RefundStatus = RefundStatus.Completed;
            refund.TransactionId = statusUpdate.TransactionId;
            refund.CompletedAt = DateTime.UtcNow;
            refund.ProcessedBy = statusUpdate.ProcessedBy;
            refund.RefunndReason = statusUpdate.RefundReason;
            //Also mark the Payment Status as Refunded
            refund.Payment.PaymentStatus = PaymentStatus.Refunded;
            _DbContext.Payments.Update(refund.Payment);
            _DbContext.Refunds.Update(refund);
            await _DbContext.SaveChangesAsync();

            // Send email notification.
            if (refund.Cancellation?.Order?.Customer != null && !string.IsNullOrEmpty(refund.Cancellation.Order.Customer.Email))
            {
                await _EmailService.SendEmailAsync(
                refund.Cancellation.Order.Customer.Email,
                $"Your Refund Has Been Processed Successfully, Order #{refund.Cancellation.Order.OrderNumber}",
                GenerateRefundSuccessEmailBody(refund, refund.Cancellation.Order.OrderNumber.ToString(), refund.Cancellation),
                IsBodyHtml: true);
            }
            var confirmation = new ConfirmationResponseDTO
            {
                Message = $"Refund with ID {refund.Id} has been updated to {refund.RefundStatus}."
            };
            return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
        }

        public async Task<ApiResponse<RefundResponseDTO>> GetRefundByIdAsync(int id)
        {
            var refund = await _DbContext.Refunds
            .Include(r => r.Cancellation)
            .ThenInclude(c => c.Order)
            .ThenInclude(o => o.Payment)
            .FirstOrDefaultAsync(r => r.Id == id);
            if (refund == null)
                return new ApiResponse<RefundResponseDTO>(404, "Refund not found.");
            return new ApiResponse<RefundResponseDTO>(200, MapRefundToDTO(refund));
        }

        // Retrieves all refunds.
        public async Task<ApiResponse<List<RefundResponseDTO>>> GetAllRefundsAsync()
        {
            var refunds = await _DbContext.Refunds
            .Include(r => r.Cancellation)
            .ThenInclude(c => c.Order)
            .ThenInclude(o => o.Payment)
            .ToListAsync();
            var refundList = refunds.Select(r => MapRefundToDTO(r)).ToList();
            return new ApiResponse<List<RefundResponseDTO>>(200, refundList);
        }

        private RefundResponseDTO MapRefundToDTO(Refund refund)
        {
            return new RefundResponseDTO
            {
                Id = refund.Id,
                CancellationId = refund.CancellationId,
                PaymentId = refund.PaymentId,
                Amount = refund.RefundAmount,
                RefundMethod = Enum.Parse<RefundMethod>(refund.RefundMethod),
                RefundReason = refund.RefunndReason,
                Status = refund.RefundStatus,
                InitiatedAt = refund.InitiatedAt,
                CompletedAt = refund.CompletedAt,
                TransactionId = refund.TransactionId
            };
        }


        // Simulates calling a payment gateway to process the refund.
        // In production, replace this with actual integration code.
        public async Task<PaymentGatewayRefundResponseDTO> ProcessRefundPaymentAsync(Refund refund)
        {
            // Simulate a network delay of 1 second.
            await Task.Delay(TimeSpan.FromSeconds(1));
            // Create a Random instance. (In production, consider reusing a static instance.)
            var random = new Random();
            double chance = random.NextDouble(); // Generates a double between 0.0 and 1.0.
            if (chance < 0.70) // 70% chance for Completed.
            {
                return new PaymentGatewayRefundResponseDTO
                {
                    IsSuccess = true,
                    Status = RefundStatus.Completed,
                    TransactionId = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
                };
            }
            else if (chance < 0.90) // Next 20% chance for Failed.
            {
                return new PaymentGatewayRefundResponseDTO
                {
                    IsSuccess = false,
                    Status = RefundStatus.Failed
                };
            }
            else // Remaining 10% chance for Pending.
            {
                return new PaymentGatewayRefundResponseDTO
                {
                    IsSuccess = false,
                    Status = RefundStatus.Pending
                };
            }
        }

        public string GenerateRefundSuccessEmailBody(Refund refund, string orderNumber, Cancellation cancellation)
        {
            // Format CompletedAt if available; otherwise show "N/A".
            // Define the IST time zone.
            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            // Convert CompletedAt from UTC to IST, if available.
            string completedAtStr = refund.CompletedAt.HasValue
            ? TimeZoneInfo.ConvertTimeFromUtc(refund.CompletedAt.Value, istZone).ToString("dd MMM yyyy HH:mm:ss")
            : "N/A";
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; margin: 0; padding: 0;'>
                <div style='background-color: #f4f4f4; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #ddd;'>
                <div style='padding: 20px; text-align: center; background-color: #2E86C1; color: #ffffff;'>
                <h2>Your Refund is Complete</h2>
                </div>
                <div style='padding: 20px;'>
                <p>Dear Customer,</p>
                <p>Your refund has been processed successfully. Below are the details:</p>
                <table style='width: 100%; border-collapse: collapse;'>
                <tr>
                <td style='border: 1px solid #ddd; padding: 8px;'>Order Number</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{orderNumber}</td>
                </tr>
                <tr>
                <td style='border: 1px solid #ddd; padding: 8px;'>Refund Transaction ID</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{refund.TransactionId}</td>
                </tr>
                <tr>
                <td style='border: 1px solid #ddd; padding: 8px;'>Order Amount</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>₹{cancellation.OrderAmount}</td>
                </tr>
                <tr>
                <td style='border: 1px solid #ddd; padding: 8px;'>Cancellation Charges</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>₹{cancellation.CancellationCharges}</td>
                </tr>
                <tr>
                <td style='border: 1px solid #ddd; padding: 8px;'>Cancellation Reason</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{cancellation.Reason}</td>
                </tr>
                <tr>
                <td style='border: 1px solid #ddd; padding: 8px;'>Refunded Method</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{refund.RefundMethod}</td>
                </tr>
                <tr>
                <td style='border: 1px solid #ddd; padding: 8px;'>Refunded Amount</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>₹{refund.RefundAmount}</td>
                </tr>
                <tr>
                <td style='border: 1px solid #ddd; padding: 8px;'>CompletedAt At</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{completedAtStr}</td>
                </tr>
                </table>
                <p>Thank you for shopping with us.</p>
                <p>Best regards,<br/>The ECommerce Team</p>
                </div>
                </div>
                </div>
                </body>
                </html>";
        }
    }
}
