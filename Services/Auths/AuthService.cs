using System;
using Microsoft.IdentityModel.Tokens;
using OptimizingLastMile.Configure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Repositories.Accounts;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace OptimizingLastMile.Services.Auths;

public class AuthService : IAuthService
{
    private readonly JwtConfig _jwtConfig;

    public AuthService(IOptionsSnapshot<JwtConfig> options)
    {
        _jwtConfig = options.Value;
    }

    public bool IsCorrectPassword(string password, string encryptPass)
    {
        return BCrypt.Net.BCrypt.Verify(password, encryptPass);
    }

    public GenericResult CheckStatusActive(StatusEnum status)
    {
        switch (status)
        {
            case StatusEnum.INACTIVE:
                {
                    var error = Errors.Auth.AccountIsDisable();
                    return GenericResult.Fail(error);
                }
            case StatusEnum.REJECTED:
                {
                    var error = Errors.Auth.AccountIsReject();
                    return GenericResult.Fail(error);
                }
            default:
                return GenericResult.Ok();
        }
    }

    public string GenerateToken(Account account)
    {
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
                new Claim("AuthRole", account.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));

        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        DateTime dateTime = DateTime.UtcNow.AddDays(30);

        var token = new JwtSecurityToken(claims: claims, expires: dateTime, signingCredentials: signIn);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        return jwtToken;
    }
}

