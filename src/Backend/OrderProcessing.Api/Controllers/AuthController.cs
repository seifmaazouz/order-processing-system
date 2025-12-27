using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs.User;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
         private readonly IAuthService _userService;

        public AuthController(IAuthService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.CreateAsync(request);
                return Created(string.Empty, user);
            }
            catch (InvalidOperationException ex)
            {
                // e.g., username already exists
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            AuthResultDto authResult = await _userService.LoginAsync(request);
            return Ok(authResult);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", detail = ex.Message });
        }
        }
    
    [HttpPost("register-admin")]
    [Authorize(Roles ="Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.CreateAdminAsync(request);
                return Created(string.Empty, user);
            }
            catch (InvalidOperationException ex)
            {
                // e.g., username already exists
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = GetBearerToken();
                var message = await _userService.LogoutAsync(token);
                return Ok(new { message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
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