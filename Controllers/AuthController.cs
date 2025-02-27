﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServicioJugosVentas.Data;
using ServicioJugosVentas.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServicioJugosVentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // Llama al stored procedure sp_LoginUsuario usando FromSqlInterpolated
            var result = await _context.LoginResults
                .FromSqlInterpolated($"EXEC sp_LoginUsuario @Username={request.Username}, @Password={request.Password}")
                .ToListAsync();
            if (result == null || result.Count == 0)
                return Unauthorized("Usuario o contraseña incorrectos.");

            var usuario = result.First();

            // Generar token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString())
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new LoginResponse { Token = tokenHandler.WriteToken(token) });
        }

        [HttpGet("test-db")]
        public async Task<IActionResult> TestDatabase()
        {
            string connectionString = "workstation id=BDJUGOS.mssql.somee.com;packet size=4096;user id=codigolixandro_SQLLogin_1;pwd=$$LixandroGomez;data source=BDJUGOS.mssql.somee.com;persist security info=False;initial catalog=BDJUGOS;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True;Connection Timeout=30";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    return Ok("✅ Conexión exitosa a la base de datos en Somee.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Error de conexión: {ex.Message}");
            }
        }
    }
}
