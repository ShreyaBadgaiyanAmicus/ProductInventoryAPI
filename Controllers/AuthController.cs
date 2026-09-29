using Microsoft.AspNetCore.Mvc;
using ProductInventoryAPI.Data;
using Microsoft.EntityFrameworkCore;
using ProductInventoryAPI.DTO;
using ProductInventoryAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace ProductInventoryAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterProductInventoryUserDTO dto)
        {
            var existingUser = await _context.ProductInventoryUsers
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            var user = new ProductInventoryUser
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _context.ProductInventoryUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginProductInventoryUserDTO dto)
        {
            var user = await _context.ProductInventoryUsers
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash
            );

            if (!passwordValid)
            {
                return Unauthorized("Invalid username or password.");
            }

           
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

           
            if (user.Role == "Admin")
            {
                claims.Add(
                    new Claim("CanManageInventory", "true")
                );
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            var refreshToken = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64)
            );

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ProductInventoryUserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = tokenString,
                refreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDTO dto)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

            if (refreshToken == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            if (refreshToken.IsRevoked)
            {
                return Unauthorized("Refresh token has been revoked.");
            }

            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized("Refresh token has expired.");
            }

            var user = await _context.ProductInventoryUsers
                .FirstOrDefaultAsync(
                    u => u.Id == refreshToken.ProductInventoryUserId
                );

            if (user == null)
            {
                return Unauthorized("User not found.");
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

       
            if (user.Role == "Admin")
            {
                claims.Add(
                    new Claim("CanManageInventory", "true")
                );
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                refreshToken = dto.RefreshToken
            });
        }
    }
}