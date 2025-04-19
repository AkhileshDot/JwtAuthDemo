public interface ITokenService
{
    Task<string> GenerateToken(string username, string role);
    Task<RefreshToken> GenerateRefreshToken(string username);
    Task<string> RefreshAccessToken(string refreshToken);
    Task<bool> RevokeRefreshToken(string refreshToken);
}
