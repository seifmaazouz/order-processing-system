using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.DTOs.User;
using OrderProcessing.Application.DTOs.Order;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.DTOs.CreditCard;

namespace OrderProcessing.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/user/orders
        [HttpGet("orders")]
        public async Task<Results<Ok<IEnumerable<CustomerOrderDto>>, UnauthorizedHttpResult>> GetPastOrders()
            {
                var token = GetBearerToken();
                var orders = await _userService.GetPastOrdersAsync(token);
            return TypedResults.Ok(orders);
        }

        // GET api/user/details
        [HttpGet("details")]
        public async Task<Results<Ok<DetailsDto>, UnauthorizedHttpResult, NotFound<ErrorResponse>>> GetDetails()
            {
                var token = GetBearerToken();
                var details = await _userService.GetDetailsAsync(token);
            return TypedResults.Ok(details);
        }

        // POST api/user/change-password
        [HttpPost("change-password")]
        public async Task<Results<Ok<string>, UnauthorizedHttpResult, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var token = GetBearerToken();
            await _userService.ChangePasswordAsync(token, request);
            return TypedResults.Ok("Password changed successfully");
        }

        // PUT api/user/profile
        [HttpPut("profile")]
        public async Task<Results<Ok, UnauthorizedHttpResult, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> UpdateProfile([FromBody] UpdateUserProfileDto request)
            {
                var token = GetBearerToken();
                await _userService.UpdateProfileAsync(token, request);
            return TypedResults.Ok();
        }

        // POST api/user/add-credit-card
        [HttpPost("add-credit-card")]
        public async Task<Results<Ok, UnauthorizedHttpResult, NotFound<ErrorResponse>, BadRequest<ErrorResponse>>> AddCreditCard([FromBody] AddCreditCardDto request)
            {
                var token = GetBearerToken();
                await _userService.AddCreditCardAsync(token, request);
            return TypedResults.Ok();
        }

        // POST api/user/remove-card
        [HttpPost("remove-card")]
        public async Task<Results<Ok, UnauthorizedHttpResult, NotFound<ErrorResponse>>> RemoveCard([FromBody] RemoveCardRequest request)
            {
                var token = GetBearerToken();
                await _userService.RemoveCreditCardAsync(request with { Token = token });
            return TypedResults.Ok();
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
