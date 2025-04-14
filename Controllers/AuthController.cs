using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenservice;

    public AuthController(TokenService tokenService)
    {
        _tokenservice = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if(request.Username == "akizone" && request.Password == "akizone")
        {
            var accessToken = _tokenservice.GenerateToken(request.Username, request.Role ?? "User");
            var refreshToken = _tokenservice.GenerateRefreshToken(request.Username);
            return Ok(new 
            {   AccessToken = accessToken,
                RefreshToken = refreshToken.Token            
            });
        }
        return Unauthorized("Invalid credentials");
    }
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenRequest request)
    {
        var newToken = _tokenservice.RefreshAccessToken(request.RefreshToken);
        if (newToken == null)
            return Unauthorized("Invalid or expired refresh token");

        return Ok(new
        {
            AccessToken = newToken,
        });
    }
    [HttpPost("logout")]
    public IActionResult Logout([FromBody] LogoutRequest request)
    {
        var result = _tokenservice.RevokeRefreshToken(request.RefreshToken);
        if (!result)
        {
            return NotFound("Refresh token not found or already revoked");
        }
        return Ok("User logged out and refresh token revoked");
    }
}

