using JwtAuthDemo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class AuthService : IAuthService
{
    private readonly DemoAppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passWordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(DemoAppDbContext dbContext, IPasswordHasher<User> hasher, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _passWordHasher = hasher;
        _tokenService = tokenService;        
    }

    public async Task<LoginResponseDto> Login(LoginRequest loginRequest)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.Username == loginRequest.Username);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid username");

        var check = _passWordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);
        if (check == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid password");

        string accesstoken = await _tokenService.GenerateToken(user.Username, user.Role);
        RefreshToken refressToken = await _tokenService.GenerateRefreshToken(user.Username);

        //return new LoginResponseDto
        //{
        //    AccessToken = accesstoken,
        //    RefreshToken = refressToken.Token,
        //    User = new UserDto
        //    {
        //        Id = user.Id,
        //        Username = user.Username,
        //        Role = user.Role
        //    }
        //};

        var result = new LoginResponseDto
        {
            AccessToken = accesstoken,
            RefreshToken = refressToken.Token,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            }
        };

        Console.WriteLine(JsonSerializer.Serialize(result)); // 👈 log it before returning
        return result;
    }

    public async Task<string> RefreshAccessToken(RefreshTokenRequest refreshTokenRequest)
    {
        string token = await _tokenService.RefreshAccessToken(refreshTokenRequest.RefreshToken);
        return token;
    }

    public async Task<bool> Logout(LogoutRequest logoutRequest)
    {
        bool result = await _tokenService.RevokeRefreshToken(logoutRequest.RefreshToken);
        return result;
    }
}
