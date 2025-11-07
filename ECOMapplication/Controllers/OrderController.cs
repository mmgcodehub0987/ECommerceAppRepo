using ECOMapplication.DTO;
using ECOMapplication.DTOs.OrderDTOs;
using ECOMapplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECOMapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _OrderService;
        // Inject the OrderService.
        public OrderController(OrderService orderService)
        {
            _OrderService = orderService;
        }

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<ApiResponse<OrderResponseDTO>>> CreateOrder([FromBody] OrderCreateDTO orderDto)
        {
            var response = await _OrderService.CreateOrderAsync(orderDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves an order by its ID.
        // GET: api/Orders/GetOrderById/{id}
        [HttpGet("GetOrderById/{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDTO>>> GetOrderById(int id)
        {
            var response = await _OrderService.GetOrderByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates the status of an existing order.
        // PUT: api/Orders/UpdateOrderStatus
        [HttpPut("UpdateOrderStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateOrderStatus([FromBody] OrderStatusUpdateDTO statusDto)
        {
            var response = await _OrderService.UpdateOrderStatusAsync(statusDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all orders.
        // GET: api/Orders/GetAllOrders
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<ApiResponse<List<OrderResponseDTO>>>> GetAllOrders()
        {
            var response = await _OrderService.GetAllOrdersAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all orders for a specific customer.
        // GET: api/Orders/GetOrdersByCustomer/{customerId}
        [HttpGet("GetOrdersByCustomer/{customerId}")]
        public async Task<ActionResult<ApiResponse<List<OrderResponseDTO>>>> GetOrdersByCustomer(int customerId)
        {
            var response = await _OrderService.GetOrdersByCustomerAsync(customerId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
