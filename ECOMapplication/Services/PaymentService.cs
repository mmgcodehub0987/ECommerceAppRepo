using ECOMapplication.DBContext;
using ECOMapplication.DTO;
using ECOMapplication.DTOs.PaymentDTOs;
using ECOMapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ECOMapplication.Services
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _DbContext;
        private readonly EmailService _EmailService;

        public PaymentService(ApplicationDbContext context, EmailService emailService)
        {
            _DbContext = context;
            _EmailService = emailService;
        }

        public async Task<ApiResponse<PaymentResponseDTO>> ProcessPaymentAsync(PaymentRequestDTO paymentRequest)
        {
            // Use a transaction to guarantee atomic operations on Order and Payment
            using (var transaction = await _DbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Retrieve the order along with any existing payment record
                    var order = await _DbContext.Orders
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.OrderId == paymentRequest.OrderId && o.CustomerId == paymentRequest.CustomerId);
                    if (order == null)
                    {
                        return new ApiResponse<PaymentResponseDTO>(404, "Order not found.");
                    }
                    if (Math.Round(paymentRequest.Amount, 2) != Math.Round(order.TotalAmount, 2))
                    {
                        return new ApiResponse<PaymentResponseDTO>(400, "Payment amount does not match the order total.");
                    }

                    // Check if an existing payment record is present
                    if (order.Payment != null)
                    {
                        // Allow retry only if previous payment failed and order status is still Pending
                        if (order.Payment.PaymentStatus == PaymentStatus.Failed && order.OrderStatus == OrderStatus.Pending)
                        {
                            // Retry: update the existing payment record with new details
                            var paymentFailed = new Payment();
                            paymentFailed = order.Payment;
                            paymentFailed.PaymentMethod = paymentRequest.PaymentMethod;
                            paymentFailed.Amount = paymentRequest.Amount;
                            paymentFailed.PaymentDate = DateTime.UtcNow;
                            paymentFailed.PaymentStatus = PaymentStatus.Pending;
                            paymentFailed.TransactionId = string.Empty; // Clear previous transaction id if any
                            _DbContext.Payments.Update(paymentFailed);
                        }
                       return new ApiResponse<PaymentResponseDTO>(400, "Order already has an associated payment.");
                    }
                    
                   // Create a new Payment record if none exists
                    var payment = new Payment()
                    { 
                        OrderId = paymentRequest.OrderId,
                        PaymentMethod = paymentRequest.PaymentMethod,
                        Amount = paymentRequest.Amount,
                        PaymentDate = DateTime.UtcNow,
                        PaymentStatus = PaymentStatus.Pending
                    };
                    _DbContext.Payments.Add(payment);
                    

                    // For non-COD payments, simulate payment processing
                    if (!IsCashOnDelivery(paymentRequest.PaymentMethod))
                    {
                        var simulatedStatus = await SimulatePaymentGateway();
                        payment.PaymentStatus = simulatedStatus;
                        if (simulatedStatus == PaymentStatus.Completed)
                        {
                            // Update the Transaction Id on successful payment
                            payment.TransactionId = GenerateTransactionId();
                            // Update order status accordingly
                            order.OrderStatus = OrderStatus.Processing;
                        }
                    }
                    else
                    {
                        // For COD, mark the order status as Processing immediately
                        order.OrderStatus = OrderStatus.Processing;
                    }
                    await _DbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    // Send Order Confirmation Email if Order Status is Processing
                    // It means the user is either selected COD of the Payment is Sucessful 
                    if (order.OrderStatus == OrderStatus.Processing)
                    {
                        await SendOrderConfirmationEmailAsync(paymentRequest.OrderId);
                    }
                    // Manual mapping to PaymentResponseDTO
                    var paymentResponse = new PaymentResponseDTO
                    {
                        PaymentId = payment.Id,
                        OrderId = payment.OrderId,
                        PaymentMethod = payment.PaymentMethod,
                        TransactionId = payment.TransactionId,
                        Amount = payment.Amount,
                        PaymentDate = payment.PaymentDate,
                        Status = payment.PaymentStatus
                    };
                    return new ApiResponse<PaymentResponseDTO>(200, paymentResponse);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return new ApiResponse<PaymentResponseDTO>(500, "An unexpected error occurred while processing the payment.");
                }
            }
        }

        private bool IsCashOnDelivery(string paymentMethod)
        {
            return paymentMethod.Equals("CashOnDelivery", StringComparison.OrdinalIgnoreCase) ||
            paymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase);
        }
        
        //payment Status generator
        private async Task<PaymentStatus> SimulatePaymentGateway()
        {
            //Simulate the PG
            await Task.Delay(TimeSpan.FromMilliseconds(1));
            int chance = Random.Shared.Next(1, 101); // 1 to 100
            if (chance <= 70)
                return PaymentStatus.Completed;
            else if (chance <= 90)
                return PaymentStatus.Pending;
            else
                return PaymentStatus.Failed;
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{Guid.NewGuid().ToString("N").ToUpper().Substring(0, 12)}";
        }

        public async Task SendOrderConfirmationEmailAsync(int orderId)
        {
            // Retrieve the order with its related customer, addresses, payment, and order items (with products)
            var order = await _DbContext.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.BillingAddress)
            .Include(o => o.ShippingAddress)
            .Include(o => o.Payment)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                // Optionally log that the order was not found.
                return;
            }
            var payment = order.Payment; // Payment details may be null if not available
                                         // Prepare the email subject.
            string subject = $"Order Confirmation - {order.OrderNumber}";
            // Build the HTML email body using string interpolation.
            string emailBody = $@"
            <html>
            <head>
            <meta charset='UTF-8'>
            </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px;'>
                    <div style='max-width: 700px; margin: auto; background-color: #ffffff; padding: 20px; border: 1px solid #dddddd;'>
            <!-- Header -->
                <div style='background-color: #007bff; padding: 15px; text-align: center; color: #ffffff;'>
                <h2 style='margin: 0;'>Order Confirmation</h2>
            </div>
<!-- Greeting and Order Details -->
<p style='margin: 20px 0 5px 0;'>Dear {order.Customer.FirstName} {order.Customer.LastName},</p>
<p style='margin: 5px 0 20px 0;'>Thank you for your order. Please find your invoice below.</p>
<table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
<tr>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Order Number:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{order.OrderNumber}</td>
</tr>
<tr>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Order Date:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{order.OrderDate:MMMM dd, yyyy}</td>
</tr>
</table>
<!-- Order Summary (placed before order items) -->
<h3 style='color: #007bff; border-bottom: 2px solid #eeeeee; padding-bottom: 5px;'>Order Summary</h3>
<table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
<tr>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Sub Total:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{order.TotalBaseAmount:C}</td>
</tr>
<tr>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Discount:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>-{order.DiscountAmoount:C}</td>
</tr>
<tr>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Shipping Cost:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{order.ShippingCost:C}</td>
</tr>
<tr style='font-weight: bold;'>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Total Amount:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{order.TotalAmount:C}</td>
</tr>
</table>
<!-- Order Items -->
<h3 style='color: #007bff; border-bottom: 2px solid #eeeeee; padding-bottom: 5px;'>Order Items</h3>
<table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
<tr style='background-color: #28a745; color: #ffffff;'>
<th style='padding: 8px; border: 1px solid #dddddd;'>Product</th>
<th style='padding: 8px; border: 1px solid #dddddd;'>Quantity</th>
<th style='padding: 8px; border: 1px solid #dddddd;'>Unit Price</th>
<th style='padding: 8px; border: 1px solid #dddddd;'>Discount</th>
<th style='padding: 8px; border: 1px solid #dddddd;'>Total Price</th>
</tr>
{string.Join("", order.OrderItems.Select(item => $@"
<tr>
<td style='padding: 8px; border: 1px solid #dddddd;'>{item.Product.ProductName}</td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{item.Quantity}</td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{item.UnitPrice:C}</td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{item.Discount:C}</td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{item.TotalPrice:C}</td>
</tr>
"))}
</table>
<!-- Addresses: Combined Billing and Shipping -->
<h3 style='color: #007bff; border-bottom: 2px solid #eeeeee; padding-bottom: 5px;'>Addresses</h3>
<table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
<tr>
<td style='width: 50%; vertical-align: top; padding: 8px; border: 1px solid #dddddd;'>
<strong>Billing Address</strong><br/>
{order.BillingAddress.AddressLine1}<br/>
{(string.IsNullOrWhiteSpace(order.BillingAddress.AddressLine2) ? "" : order.BillingAddress.AddressLine2 + "<br/>")}
{order.BillingAddress.City}, {order.BillingAddress.State} {order.BillingAddress.PostalCode}<br/>
{order.BillingAddress.Country}
</td>
<td style='width: 50%; vertical-align: top; padding: 8px; border: 1px solid #dddddd;'>
<strong>Shipping Address</strong><br/>
{order.ShippingAddress.AddressLine1}<br/>
{(string.IsNullOrWhiteSpace(order.ShippingAddress.AddressLine2) ? "" : order.ShippingAddress.AddressLine2 + "<br/>")}
{order.ShippingAddress.City}, {order.ShippingAddress.State} {order.ShippingAddress.PostalCode}<br/>
{order.ShippingAddress.Country}
</td>
</tr>
</table>
<!-- Payment Details -->
<h3 style='color: #007bff; border-bottom: 2px solid #eeeeee; padding-bottom: 5px;'>Payment Details</h3>
<table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
<tr>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Payment Method:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{(payment != null ? payment.PaymentMethod : "N/A")}</td>
</tr>
<tr>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Payment Date:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{(payment != null ? payment.PaymentDate.ToString("MMMM dd, yyyy HH:mm") : "N/A")}</td>
</tr>
<tr>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Transaction ID:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{(payment != null ? payment.TransactionId : "N/A")}</td>
</tr>
<tr>
<td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Status:</strong></td>
<td style='padding: 8px; border: 1px solid #dddddd;'>{(payment != null ? payment.PaymentStatus.ToString() : "N/A")}</td>
</tr>
</table>
<p style='margin-top: 20px;'>If you have any questions, please contact our support team.</p>
<p>Best regards,<br/>Your E-Commerce Store Team</p>
</div>
</body>
</html>";
            // Send the email using the EmailService.
            await _EmailService.SendEmailAsync(order.Customer.Email, subject, emailBody, IsBodyHtml: true);
        }

    }

}
