using Microsoft.AspNetCore.Mvc;

namespace Artix.API.Endpoints.Controllers;

using System.Security.Claims;
using _primitives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : BaseController
{
    public AuthenticationController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("register")]
    
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized();

        var token = GenerateJwtToken(user); // You write this method

        // store token in AspNetUserTokens
        await _userManager.SetAuthenticationTokenAsync(user, "ArtixApp", "access_token", token);

        return Ok(new { token });
    }

    
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "access_token");

        return Ok(new { message = "Logged out successfully." });
    }

    
    
    
    [HttpGet("profile")]
    
    [HttpPatch("profile")]
}
