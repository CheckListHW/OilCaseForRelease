using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OilCaseApi.Models;

namespace OilCaseApi.Controllers.Api.UserData.Authorization;

public class JwtGenerator: IJwtGenerator
{
    private const string TokenKey = "asdfasdfasdfasdfasdfsadgshytrfdhjd";
    private readonly SymmetricSecurityKey _key;

    public string CreateToken(User user)
    {
        var claims = new List<Claim> { new(JwtRegisteredClaimNames.NameId, user.UserName) };
        var credentials = new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(7),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(TokenKey));
}