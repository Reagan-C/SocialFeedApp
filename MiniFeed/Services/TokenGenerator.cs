using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniFeed.Interfaces;
using MiniFeed.Models;
using MiniFeed.Models.jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniFeed.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtOptions _jWtOptions;

        public TokenGenerator(IOptions<JwtOptions> jWtOptions)
        {
            _jWtOptions=jWtOptions.Value;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWtOptions.Secret));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>
            {
                new Claim (JwtRegisteredClaimNames.Sub, user.Id),
                new Claim (JwtRegisteredClaimNames.Email, user.Email),
                new Claim (JwtRegisteredClaimNames.Name, user.UserName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jWtOptions.Issuer,
                Audience = _jWtOptions.Audience,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = signingCredentials

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
