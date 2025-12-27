using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService _adminOrderService;

        public AdminOrderController(IAdminOrderService adminOrderService)
        {
            _adminOrderService = adminOrderService;
        }

        [HttpPost]
        public async Task<ActionResult<int>> PlacePublisherOrder([FromBody] PlacePublisherOrderRequest request)
        {
            var username = User.Identity?.Name ?? throw new UnauthorizedAccessException("User not authenticated");
            
            var orderId = await _adminOrderService.PlacePublisherOrderAsync(username, request.PublisherId, request.Items);
            return Ok(new { OrderId = orderId });
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminOrderDto>>> GetAllOrders()
        {
            var orders = await _adminOrderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<AdminOrderDto>> GetOrderById(int orderId)
        {
            var order = await _adminOrderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound($"Order {orderId} not found");

            return Ok(order);
        }

        [HttpPut("{orderId}/status")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            await _adminOrderService.UpdateOrderStatusAsync(orderId, request.Status);
            return NoContent();
        }
    }
}
