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
            var Token = _tokenservice.GenerateToken(request.Username);
            return Ok(new { Token = Token });
        }
        return Unauthorized("Invalid credentials");
    }
}

