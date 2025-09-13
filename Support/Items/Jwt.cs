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

    // Returns ClaimsPrincipal if valid; otherwise null
    public ClaimsPrincipal? GetPrincipalFromToken(string token, bool validateLifetime = true)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "GestionExpedientesAPI",
                ValidAudience = "GestionExpedientesCliente",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    configuration["JwtSettings:SecretKey"] ?? ""
                ))
            };
            var cleanToken = token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();
            var principal = tokenHandler.ValidateToken(cleanToken, validationParameters, out SecurityToken _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    // Convenience: obtain username (sub claim) from Authorization header
    public string? GetUsernameFromAuthorization(string authorization)
    {
        var principal = GetPrincipalFromToken(authorization);
        if (principal == null) return null;

        // Prefer the 'sub' claim used at token generation
        var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!string.IsNullOrWhiteSpace(sub)) return sub;

        // Fallbacks if mapping differs
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? principal.Identity?.Name;
    }
}

public class JwtResponse
{
    public string Token { get; set; } = String.Empty;
    public DateTime Expire { get; set; }
    public int AutoRefreshInSeconds { get; set; }
}
