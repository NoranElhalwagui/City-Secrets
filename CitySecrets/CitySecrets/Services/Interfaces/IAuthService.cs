using CitySecrets.DTOs;

namespace CitySecrets.Services.Interfaces
{
    public interface IAuthService
    {
        AuthResult Register(RegisterRequest request);
        AuthResult Login(LoginRequest request);
        AuthResult RefreshToken(string refreshToken);
        bool VerifyEmail(string token);
        void ForgotPassword(string email);
        bool ResetPassword(ResetPasswordRequest request);
        
        // User Profile Management
        UserDto? GetCurrentUser(int userId);
        bool UpdateProfile(int userId, UpdateProfileRequest request);
        bool ChangePassword(int userId, ChangePasswordRequest request);
    }
}
