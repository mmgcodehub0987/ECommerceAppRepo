using ECOMapplication.DBContext;
using ECOMapplication.DTO;
using ECOMapplication.DTOs.OrderDTOs;
using ECOMapplication.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ECOMapplication.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _DbContext;

        public OrderService(ApplicationDbContext applicationDbContext)
        {
            _DbContext = applicationDbContext;
        }
        private static readonly Dictionary<OrderStatus, List<OrderStatus>> AllowedStatusTransitions = new()
        {
            { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Canceled } },
            { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Canceled } },
            { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
            { OrderStatus.Delivered, new List<OrderStatus>() }, // Terminal state
            { OrderStatus.Canceled, new List<OrderStatus>() }   // Terminal state
        };

        public async Task<ApiResponse<OrderResponseDTO>> CreateOrderAsync(OrderCreateDTO orderDto)
        {
            try
            {
                // Validate that the customer exists.
                var customer = await _DbContext.Customers.FindAsync(orderDto.CustomerId);
                if (customer == null)
                {
                    return new ApiResponse<OrderResponseDTO>(404, "Customer does not exist.");
                }

                // Validate that the billing address exists and belongs to the customer.
                var billingAddress = await _DbContext.Addresses.FindAsync(orderDto.BillingAddressId);
                if (billingAddress == null || billingAddress.CustomerId != orderDto.CustomerId)
                {
                    return new ApiResponse<OrderResponseDTO>(400, "Billing Address is invalid or does not belong to the customer.");
                }

                // Validate that the shipping address exists and belongs to the customer.
                var shippingAddress = await _DbContext.Addresses.FindAsync(orderDto.ShippingAddressId);
                if (shippingAddress == null || shippingAddress.CustomerId != orderDto.CustomerId)
                {
                    return new ApiResponse<OrderResponseDTO>(400, "Shipping Address is invalid or does not belong to the customer.");
                }
                // Initialize financial tracking.
                decimal totalBaseAmount = 0;
                decimal totalDiscountAmount = 0;
                decimal shippingCost = 10.00m; // Example fixed shipping cost.
                decimal totalAmount = 0;
                // Generate a unique order number.
                int orderNumber = GenerateOrderNumber();
                // List to hold order items.
                var orderItems = new List<OrderItem>();
                // Process each order item from the DTO.
                foreach (var itemDto in orderDto.OrderItems)
                {
                    // Check if the product exists.
                    var product = await _DbContext.Products.FindAsync(itemDto.ProductId);
                    if (product == null)
                    {
                        return new ApiResponse<OrderResponseDTO>(404, $"Product with ID {itemDto.ProductId} does not exist.");
                    }
                    // Check if sufficient stock is available.
                    if (product.StockQuantity < itemDto.Quantity)
                    {
                        return new ApiResponse<OrderResponseDTO>(400, $"Insufficient stock for product {product.ProductName}.");
                    }
                    // Calculate base price, discount, and total price for the order item.
                    decimal basePrice = itemDto.Quantity * product.Price;
                    decimal discount = (product.DiscountPercentage / 100.0m) * basePrice;
                    decimal totalPrice = basePrice - discount;
                    // Create a new OrderItem.
                    var orderItem = new OrderItem
                    {
                        ProdctId = product.Id,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.Price,
                        Discount = discount,
                        TotalPrice = totalPrice
                    };
                    // Add the order item to the list.
                    orderItems.Add(orderItem);
                    // Update the running totals.
                    totalBaseAmount += basePrice;
                    totalDiscountAmount += discount;
                    // Deduct the purchased quantity from the product’s stock.
                    product.StockQuantity -= itemDto.Quantity;
                    _DbContext.Products.Update(product);
                }
                // Calculate the final total amount.
                totalAmount = totalBaseAmount - totalDiscountAmount + shippingCost;
                // Manually map from DTO to Order model.
                var order = new Order
                {
                    OrderNumber = orderNumber,
                    CustomerId = orderDto.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    BillingAddressId = orderDto.BillingAddressId,
                    ShippingAddressId = orderDto.ShippingAddressId,
                    TotalBaseAmount = totalBaseAmount,
                    DiscountAmoount = totalDiscountAmount,
                    ShippingCost = shippingCost,
                    TotalAmount = totalAmount,
                    OrderStatus = OrderStatus.Pending,
                    OrderItems = orderItems
                };
                // Add the order to the database.
                _DbContext.Orders.Add(order);
                // Mark the customer's active cart as checked out (if it exists).
                var cart = await _DbContext.Carts.FirstOrDefaultAsync(c => c.CustomerId == orderDto.CustomerId && !c.IsCheckedOut);
                if (cart != null)
                {
                    cart.IsCheckedOut = true;
                    cart.UpdatedAt = DateTime.UtcNow;
                    _DbContext.Carts.Update(cart);
                }
                // Save all changes.
                await _DbContext.SaveChangesAsync();
                // Map the saved order to OrderResponseDTO.
                var orderResponse = MapOrderToDTO(order, customer, billingAddress, shippingAddress);
                return new ApiResponse<OrderResponseDTO>(200, orderResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<OrderResponseDTO>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                // Retrieve the order with its items, customer, and addresses details.
                var order = await _DbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .Include(o => o.BillingAddress)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    return new ApiResponse<OrderResponseDTO>(404, "Order not found.");
                }
                // Map the order to a DTO.
                var orderResponse = MapOrderToDTO(order, order.Customer, order.BillingAddress, order.ShippingAddress);
                return new ApiResponse<OrderResponseDTO>(200, orderResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateOrderStatusAsync(OrderStatusUpdateDTO statusDto)
        {
            try
            {
                // Retrieve the order.
                var order = await _DbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == statusDto.OrderId);
                if (order == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Order not found.");
                }
                var currentStatus = order.OrderStatus;
                var newStatus = statusDto.OrderStatus;
                // Validate the status transition.
                if (!AllowedStatusTransitions.TryGetValue(currentStatus, out var allowedStatuses))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(500, "Current order status is invalid.");
                }
                if (!allowedStatuses.Contains(newStatus))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, $"Cannot change order status from {currentStatus} to {newStatus}.");
                }
                // Update the order status.
                order.OrderStatus = newStatus;
                await _DbContext.SaveChangesAsync();
                // Prepare a confirmation message.
                var confirmation = new ConfirmationResponseDTO
                {
                    Message = $"Order Status with Id {statusDto.OrderId} updated successfully."
                };
                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<OrderResponseDTO>>> GetAllOrdersAsync()
        {
            try
            {
                // Retrieve all orders with related entities.
                var orders = await _DbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .Include(o => o.BillingAddress)
                .Include(o => o.ShippingAddress)
                .AsNoTracking()
                .ToListAsync();
                // Map each order to its corresponding DTO.
                var orderList = orders.Select(o => MapOrderToDTO(o, o.Customer, o.BillingAddress, o.ShippingAddress)).ToList();
                return new ApiResponse<List<OrderResponseDTO>>(200, orderList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<OrderResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
        // Retrieves all orders associated with a specific customer.
        public async Task<ApiResponse<List<OrderResponseDTO>>> GetOrdersByCustomerAsync(int customerId)
        {
            try
            {
                // Retrieve the customer along with their orders and related data.
                var customer = await _DbContext.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(c => c.Adresses)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customerId);
                if (customer == null)
                {
                    return new ApiResponse<List<OrderResponseDTO>>(404, "Customer not found.");
                }
                // Map each order to a DTO.
                var orders = customer.Orders.Select(o => MapOrderToDTO(o, customer, o.BillingAddress, o.ShippingAddress)).ToList();
                return new ApiResponse<List<OrderResponseDTO>>(200, orders);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<OrderResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        private OrderResponseDTO MapOrderToDTO(Order order, Customer customer, Address billingAddress, Address shippingAddress)
        {
            // Map order items.
            var orderItemsDto = order.OrderItems.Select(oi => new OrderItemResponseDTO
            {
                Id = oi.Id,
                ProductId = oi.ProdctId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Discount = oi.Discount,
                TotalPrice = oi.TotalPrice
            }).ToList();
            // Create and return the DTO.
            return new OrderResponseDTO
            {
                Id = order.OrderId,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                BillingAddressId = order.BillingAddressId,
                ShippingAddressId = order.ShippingAddressId,
                TotalBaseAmount = order.TotalBaseAmount,
                TotalDiscountAmount = order.DiscountAmoount,
                ShippingCost = order.ShippingCost,
                TotalAmount = Math.Round(order.TotalAmount, 2),
                OrderStatus = order.OrderStatus,
                OrderItems = orderItemsDto
            };
        }
        // Generates a unique order number using the current UTC date/time and a random number.
        // Format: ORD-yyyyMMdd-HHmmss-XXXX
        private int GenerateOrderNumber()
        {
            Random random = new Random();
            int randomPart = random.Next(1000, 9999);
            int timePart = DateTime.Now.Millisecond;
            return int.Parse($"{timePart}{randomPart}");
        }
        // Generates a random number between min and max.
        private int RandomNumber(int min, int max)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return Math.Abs(BitConverter.ToInt32(bytes, 0) % (max - min + 1)) + min;
            }
        }

    }
    
}
