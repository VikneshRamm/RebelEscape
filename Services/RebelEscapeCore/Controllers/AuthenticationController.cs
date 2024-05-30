using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RebelEscapeCore.Models;

namespace RebelEscapeCore.Controllers
{
    [ApiController]
    [Route("/auth/")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel loginModel)
        {
            LoginSuccessResponseModel model = new LoginSuccessResponseModel();
            model.UserId = Guid.NewGuid().ToString();
            model.Token = GenerateToken(loginModel, model.UserId);
            return Ok(model);
        }

        private string GenerateToken(LoginModel user, string userId)
        {
            string? jwtTokenKey = Environment.GetEnvironmentVariable("JWT_TOKEN_KEY");
            if (string.IsNullOrEmpty(jwtTokenKey))
            {
                throw new Exception("JWT Token Key Not Available");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("userId", userId),
            };

            SecurityTokenDescriptor tokenDescriptor = new ()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = credentials,
            };

            JwtSecurityTokenHandler tokenHandler = new ();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
