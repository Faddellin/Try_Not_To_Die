using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Try_not_to_DIE.Configuration;
using Try_not_to_DIE.Models.Doctor;
using Microsoft.Extensions.Options;
using System.Text;

namespace Try_not_to_DIE.Services
{
    public class JwtService
    {

        private readonly IOptions<AppConfig> _config;

        public JwtService(IOptions<AppConfig> config)
        {
            _config = config;
        }

        public string GenerateToken(DoctorDB user)
        {
            var claims = new List<Claim>()
            {
                new Claim("Id", user.id.ToString())
            };

            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,ClaimsIdentity.DefaultRoleClaimType);

            var jwtToken = new JwtSecurityToken(issuer: _config.Value.authConfig.ISSUER,
                                                audience: _config.Value.authConfig.AUDIENCE,
                                                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
                                                claims: claimsIdentity.Claims,
                                                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.Value.authConfig.KEY))
                                                , SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public Guid GetIdFromToken(string tokenStr)
        {
            var token = new JwtSecurityToken(jwtEncodedString: tokenStr);

            return new Guid(token.Claims.First(o => o.Type == "Id").Value);

        }
    }
    
}
