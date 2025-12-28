using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.DTOs.Responses;
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


        [HttpGet]
        public async Task<Ok<List<AdminOrderDto>>> GetAllOrders()
        {
            var orders = await _adminOrderService.GetAllOrdersAsync();
            return TypedResults.Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<Results<Ok<AdminOrderDto>, NotFound<ErrorResponse>>> GetOrderById(int orderId)
        {
            var order = await _adminOrderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return TypedResults.NotFound(new ErrorResponse($"Order {orderId} not found.", 404));

            return TypedResults.Ok(order);
        }

        [HttpPut("{orderId}/status")]
        public async Task<Results<NoContent, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var username = User.Identity?.Name
                ?? throw new UnauthorizedAccessException("User not authenticated");

            await _adminOrderService.UpdateOrderStatusAsync(orderId, request.Status, username);
            return TypedResults.NoContent();
        }
    }
}
