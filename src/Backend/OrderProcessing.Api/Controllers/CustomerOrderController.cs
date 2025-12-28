using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // require authentication
    public class CustomerOrderController : ControllerBase
    {
        private readonly ICustomerOrderService _orderService;

        public CustomerOrderController(ICustomerOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/customerorder/my
        [HttpGet("my")]
        public async Task<Results<Ok<IReadOnlyList<CustomerOrderDto>>, UnauthorizedHttpResult>> GetMyOrders()
        {
            // Get the token from the Authorization header
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader is null || !authHeader.StartsWith("Bearer "))
                throw new UnauthorizedAccessException("Missing or invalid Authorization header.");

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var orders = await _orderService.GetMyOrdersAsync(token);

            return TypedResults.Ok(orders);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        public async Task<Results<Created<CustomerOrderDto>, BadRequest<ErrorResponse>, UnauthorizedHttpResult>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var order = await _orderService.CreateOrderAsync(token, request);
            return TypedResults.Created($"/api/customerorder/my", order);
        }
    }
}
