using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs.Order;
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

        // GET: api/orders/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                // Get the token from the Authorization header
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader is null || !authHeader.StartsWith("Bearer "))
                    return Unauthorized(new { message = "Missing or invalid Authorization header." });

                var token = authHeader.Substring("Bearer ".Length).Trim();

                var orders = await _orderService.GetMyOrdersAsync(token);

                return Ok(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", detail = ex.Message });
            }
        }
    }
}
