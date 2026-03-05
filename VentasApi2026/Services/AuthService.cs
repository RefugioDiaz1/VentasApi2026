using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VentasApi2026.DTOs;
using VentasApi2026.Exceptions;
using VentasApi2026.Migrations;
using VentasApi2026.Models;
using VentasApi2026.Repositories.Interfaces;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher _PassHandler;
        public AuthService(IUnitOfWork unitOfWork, IConfiguration config, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _PassHandler = passwordHasher;
        }

        // TODO: Implementar Redis blacklist para invalidación granular de tokens
        // Referencia: JTI claim + Redis con TTL igual al AccessToken


        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        { 
            var user = await _unitOfWork.Users.GetByUsernameAsync(dto.Username);

            if (user == null || !_PassHandler.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid credentials");

            var permissions = await _unitOfWork.Users.GetPermissionsAsync(user.Id);

            var role = await _unitOfWork.Users.GetRoleNameByUserIdAsync(user.Id);

            var accessToken = GenerateToken(user, permissions, role);

            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id

            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };

        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _unitOfWork.RefreshTokens.GetActiveTokenAsync(refreshToken);
            if (token == null)
                throw new NotFoundException("Token not Found");

            token.IsRevoked = true;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AuthResponseDto> RefreshAsync(TokenRefreshRequest request)
        {
            var refreshToken = await _unitOfWork.RefreshTokens
                .GetActiveTokenAsync(request.RefreshToken);

            if (refreshToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            refreshToken.User.TokenVersion++;
            refreshToken.IsRevoked = true;

            // ✅ Carga permisos y los pasa
            var permissions = await _unitOfWork.Users.GetPermissionsAsync(refreshToken.User.Id);

            var role = await _unitOfWork.Users.GetRoleNameByUserIdAsync(refreshToken.Id);

            // Genera nuevo AccessToken
            var newAccessToken = GenerateToken(refreshToken.User, permissions, role);

            // Opcional — rota el RefreshToken
            refreshToken.IsRevoked = true;
            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = refreshToken.UserId
            };

            await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        private string GenerateToken(User user, List<string> permissions, string role)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, role),
                new Claim("tokenVersion", user.TokenVersion.ToString()) // ✅ nuevo
            };

            // ✅ Agrega cada permiso como claim
            foreach (var permission in permissions)
                claims.Add(new Claim("permission", permission));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    double.Parse(jwtSettings["DurationInMinutes"]!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task RegisterAsync(RegisterUserDto dto,int idUser)
        {
            var existingUser = await _unitOfWork.Users
                .GetByUsernameAsync(dto.Username);

            if (existingUser != null)
                throw new ConflictException("Username already exists");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = _PassHandler.Hash(dto.Password),
                CreatedByUserId = idUser
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

    }
}
