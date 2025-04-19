using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = _authService.Login(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized("Invalid credentials");
        }

        
        
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var newToken = await _authService.RefreshAccessToken(request);
        if (newToken == null)
            return Unauthorized("Invalid or expired refresh token");

        return Ok(new
        {
            AccessToken = newToken,
        });
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var result = await _authService.Logout(request);
        if (!result)
        {
            return NotFound("Refresh token not found or already revoked");
        }
        return Ok("User logged out and refresh token revoked");
    }
}

