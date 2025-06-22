using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eVote.src.Model;
using eVote.src.Model.DTO;
using Microsoft.IdentityModel.Tokens;

namespace eVote.src.Service
{
    public class JwtToken
    {
        public string GenerateToken(string userId, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //TODO store secret key in a global var accessible only server side
            var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsLongEnoughAndRandom");

            var claims = new List<Claim>
            {
                new Claim("Id", userId),
                new Claim("Email", email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1f),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsLongEnoughAndRandom");

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
