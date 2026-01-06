using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.DTOs.User;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
         private readonly IAuthService _userService;

        public AuthController(IAuthService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<Results<Created<UserDto>, BadRequest<ErrorResponse>, Conflict<ErrorResponse>>> Register([FromBody] CreateUserRequest request)
            {
                var user = await _userService.CreateAsync(request);
            return TypedResults.Created($"/api/user/{user.Username}", user);
            }

        [HttpPost("login")]
        public async Task<Results<Ok<AuthResultDto>, BadRequest<ErrorResponse>, UnauthorizedHttpResult>> Login([FromBody] LoginRequest request)
        {
            var authResult = await _userService.LoginAsync(request);
            return TypedResults.Ok(authResult);
        }
    
    [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<Results<Created<UserDto>, BadRequest<ErrorResponse>, Conflict<ErrorResponse>>> RegisterAdmin([FromBody] CreateUserRequest request)
            {
                var user = await _userService.CreateAdminAsync(request);
            return TypedResults.Created($"/api/user/{user.Username}", user);
        }

        [HttpPost("logout")]
        public async Task<Ok> Logout()
            {
                var token = GetBearerToken();
            await _userService.LogoutAsync(token);
            return TypedResults.Ok();
        }

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