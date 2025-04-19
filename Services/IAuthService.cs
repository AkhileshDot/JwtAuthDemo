public interface IAuthService
{
    Task<LoginResponseDto> Login(LoginRequest loginRequest);
    Task<string> RefreshAccessToken(RefreshTokenRequest refreshTokenRequest);
    Task<bool> Logout(LogoutRequest logoutRequest);
}
