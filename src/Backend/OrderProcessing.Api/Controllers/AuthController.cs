using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs.User;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
         private readonly IUserService _userService;

        public AuthController(IUserService userService)
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
                return CreatedAtAction(nameof(Register), new { username = user.Username }, user);
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
                var user = await _userService.CreateAsync(request);
                return CreatedAtAction(nameof(Register), new { username = user.Username }, user);
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
    }
}