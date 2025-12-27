using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs.User;
using OrderProcessing.Application.DTOs.Requests;
using OrderProcessing.Application.Interfaces;
using System.Security.Authentication;

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

        // GET api/user/details
        [HttpGet("details")]
        public async Task<IActionResult> GetDetails()
        {
            try
            {
                var token = GetBearerToken();
                var details = await _userService.GetDetailsAsync(token);
                return Ok(details);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST api/user/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var token = GetBearerToken();
                await _userService.ChangePasswordAsync(request with { Token = token });
                return Ok(new { message = "Password changed successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST api/user/remove-card
        [HttpPost("remove-card")]
        public async Task<IActionResult> RemoveCard([FromBody] RemoveCardRequest request)
        {
            try
            {
                var token = GetBearerToken();
                await _userService.RemoveCreditCardAsync(request with { Token = token });
                return Ok(new { message = "Credit card removed successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
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
