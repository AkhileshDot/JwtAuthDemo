using AutoMapper;
using JwtAuthDemo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class AuthService : IAuthService
{
    private readonly DemoAppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passWordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(DemoAppDbContext dbContext, IPasswordHasher<User> hasher, ITokenService tokenService, IMapper mapper)
    {
        _dbContext = dbContext;
        _passWordHasher = hasher;
        _tokenService = tokenService;
        _mapper = mapper; 
    }

    public async Task<LoginResponseDto> Login(LoginRequest loginRequest)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.Username == loginRequest.Username);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid username");

        var check = _passWordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);
        if (check == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid password");

        string accessToken = await _tokenService.GenerateToken(user.Username, user.Role);
        RefreshToken refressToken = await _tokenService.GenerateRefreshToken(user.Username);

        return _mapper.Map<LoginResponseDto>((user, accessToken, refressToken));
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
