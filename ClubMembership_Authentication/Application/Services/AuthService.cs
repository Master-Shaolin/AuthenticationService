using ClubMembership_Authentication.API.DTOs;
using ClubMembership_Authentication.Domain.Common;
using ClubMembership_Authentication.Domain.Entities;
using ClubMembership_Authentication.Infrastructure.Repositories;
using dotenv.net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClubMembership_Authentication.Application.Services
{
    public class AuthService(IUserRepository userRepository, IConfiguration configuration) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _configuration = configuration;

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new Exception("Passwords do not match.");
            }

            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists.");
            }

            // Get the "User" role from the database
            var userRole = await _userRepository.GetUserRoleAsync("User") ?? throw new Exception("Default user role not found.");
            var hashedPassword = Utils.HashPassword(registerDto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = registerDto.Name,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = registerDto.PhoneNumber,
                RoleId = userRole.Id, // Always assign regular user role
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);

            return "User registered successfully.";
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Invalid credentials.");
            }
            return GenerateJwtToken(user);
        }

        private static string GenerateJwtToken(User user)
        {
            var Environment = DotEnv.Read();

            var secretKey = Environment["JWT_SECRET_KEY"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty.");
            }

            var issuer = Environment["JWT_ISSUER"];
            var audience = Environment["JWT_AUDIENCE"];
            var expirationMinutes = int.Parse(Environment["JWT_EXPIRATION_DELTA"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            return Utils.HashPassword(password) == hashedPassword;
        }
    }
}
