using LibraryManagement.Application.IRepositories;
using LibraryManagement.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.WebAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationService _authService;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IAuthenticationService authService = null)
        {
            _next = next;
            _configuration = configuration;
            _authService = authService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, token);

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "uid").Value;

                if (_authService != null)
                {
                    // Use the provided authentication service to retrieve the user
                    var user = await _authService.GetByIdAsync(userId);
                    context.Items["User"] = user;
                }
                else
                {
                    // For testing purposes, you can create a mock user
                    var mockUser = new User
                    {
                        Id = userId,
                        // Set other user properties as needed
                    };
                    context.Items["User"] = mockUser;
                }
            }
            catch
            {
                // Log exception or handle token validation failure
            }
        }
    }
}
