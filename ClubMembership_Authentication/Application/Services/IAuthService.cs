using ClubMembership_Authentication.API.DTOs;

namespace ClubMembership_Authentication.Application.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
    }
}
