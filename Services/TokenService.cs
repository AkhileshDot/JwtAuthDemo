using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
public class TokenService
{
    private readonly JwtSettings _jwtSettings;
    private static List<RefreshToken> refreshTokens = new List<RefreshToken>();
    public TokenService(IOptions<JwtSettings> jwtsettings)
    {
        _jwtSettings = jwtsettings.Value;
    }

    public string GenerateToken(string username, string role)
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
    public RefreshToken GenerateRefreshToken(string username)
    {
        var token = new RefreshToken
        {
            Username = username,
            Token = Guid.NewGuid().ToString(),
            ExpiryTime = DateTime.UtcNow.AddDays(7)
        };
        refreshTokens.Add(token);
        return token;
    }
    public string RefreshAccessToken(string refreshToken)
    {
        var token = refreshTokens.SingleOrDefault(x => x.Token == refreshToken);

        if(token == null || token.ExpiryTime <= DateTime.UtcNow)
                return null;
        return GenerateToken(token.Username, "Admin");


    }
    public bool RevokeRefreshToken(string refreshToken)
    {
        var token = refreshTokens.SingleOrDefault(x => x.Token == refreshToken);
        if(token != null)
        {
            refreshTokens.Remove(token);
            return true;
        }
        return false;

    }
}

