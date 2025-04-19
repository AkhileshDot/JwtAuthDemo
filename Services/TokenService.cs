using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuthDemo.Data;
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private static List<RefreshToken> refreshTokens = new List<RefreshToken>();
    private readonly DemoAppDbContext _dbContext;
    public TokenService(IOptions<JwtSettings> jwtsettings, DemoAppDbContext dbContext)
    {
        _jwtSettings = jwtsettings.Value;
        _dbContext = dbContext;
    }

    public async Task<string> GenerateToken(string username, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<RefreshToken> GenerateRefreshToken(string username)
    {
        var token = new RefreshToken
        {
            Username = username,
            Token = Guid.NewGuid().ToString(),
            ExpiryTime = DateTime.UtcNow.AddDays(7)
        };

        _dbContext.RefreshTokens.Add(token);
        _dbContext.SaveChanges();

        return token;
    }
    public async Task<string> RefreshAccessToken(string refreshToken)
    {
        var token = _dbContext.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

        if(token == null || token.ExpiryTime <= DateTime.UtcNow)
                return null;
        return await GenerateToken(token.Username, "Admin");


    }
    public async Task<bool> RevokeRefreshToken(string refreshToken)
    {
        var token = _dbContext.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
        if(token != null)
        {
            _dbContext.RefreshTokens.Remove(token);
            _dbContext.SaveChanges();
            return true;
        }
        return false;

    }
}

