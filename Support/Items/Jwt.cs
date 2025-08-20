using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Support.Items;

public class Jwt
{
    private readonly IConfiguration configuration;
    private readonly int minutesLiveToken = 1;
    public Jwt(IConfiguration _configuration)
    {
        configuration = _configuration;
        minutesLiveToken = Convert.ToInt32(configuration["JwtSettings:MinutesLiveToken"]);
    }

    public JwtResponse GenerateJwtToken(string? username)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration["JwtSettings:SecretKey"] ?? ""
        ));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expireAt = DateTime.UtcNow.AddMinutes(minutesLiveToken);

        var token = new JwtSecurityToken(
            issuer: "GestionExpedientesAPI",
            audience: "GestionExpedientesCliente",
            claims: claims,
            expires: expireAt,
            signingCredentials: credentials
        );

        return new JwtResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expire = expireAt,
            AutoRefreshInSeconds = (int)(minutesLiveToken * 60) - 60
        };
    }
    /*************  ✨ Windsurf Command ⭐  *************/
    /// <summary>
    /// Validates the JWT token against the secret key in the config.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>true if the token is valid, false otherwise.</returns>
    /*******  9ceb5829-ee34-4ebe-8f3c-3a6f156530a9  *******/
    public bool ValidateJwtToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "GestionExpedientesAPI",
                ValidAudience = "GestionExpedientesCliente",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    configuration["JwtSettings:SecretKey"] ?? ""
                ))
            };
            var cleanToken = token.Replace("Bearer ", "").Trim();
            tokenHandler.ValidateToken(cleanToken, validationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class JwtResponse
{
    public string Token { get; set; } = String.Empty;
    public DateTime Expire { get; set; }
    public int AutoRefreshInSeconds { get; set; }
}