using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOS.User;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController:ControllerBase
    {
         private readonly IUserService _userService;

        public UserController(IUserService userService)
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
                // unexpected errors
                return StatusCode(500, new { message = ex.Message });
            }
        }
        
    }
}