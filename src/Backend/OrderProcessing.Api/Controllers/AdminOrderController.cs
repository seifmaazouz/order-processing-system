using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Security;

namespace OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService _adminOrderService;
        private readonly IJwtService _jwtService;

        public AdminOrderController(IAdminOrderService adminOrderService, IJwtService jwtService)
        {
            _adminOrderService = adminOrderService;
            _jwtService = jwtService;
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
        public async Task<Results<NoContent, NotFound<ErrorResponse>, BadRequest<ErrorResponse>, ForbidHttpResult>> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var token = GetBearerToken();
            var username = _jwtService.GetUsernameFromToken(token);
            await _adminOrderService.UpdateOrderStatusAsync(orderId, request.Status, username);
            return TypedResults.NoContent();
        }

        // Helper: Extract Bearer token from header
        private string GetBearerToken()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
                throw new UnauthorizedAccessException("Authorization header missing");

            var token = authHeader.ToString().Replace("Bearer ", "").Trim();
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("Bearer token missing");

            return token;
        }
    }
}
