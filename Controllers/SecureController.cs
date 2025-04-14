using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("Api/[controller]")]
public class SecureController : Controller
{
    [HttpGet("secret")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetSecret()
    {
        var username = UserStore.users[1].Username;
        return Ok($"🔒 This is a protected message for {username}");
    }
}
