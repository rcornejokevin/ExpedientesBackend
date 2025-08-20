using Microsoft.AspNetCore.Mvc;
using ApiHandler.Models.Security;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using DBHandler.Service.Security;
namespace ApiHandler.Controllers.Security;

[ApiController]
[Route("seguridad")]
public class LoginController : ControllerBase
{
    private readonly Jwt jwt;
    private readonly AuthService authService;
    public LoginController(Jwt _jwt, AuthService _authService)
    {
        jwt = _jwt;
        authService = _authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        ResponseApi response = new ResponseApi();
        try
        {
            if (await authService.LoginSuccessAsync(request.username, request.password) == null)
            {
                response.code = "999";
                response.message = "El usuario y la contraseña son incorrectos";
                return Unauthorized(response);
            }
            var token = jwt.GenerateJwtToken(request.username);
            response.code = "000";
            response.message = "Login successful";
            response.data = new { token.Token, token.Expire };
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.code = "500";
            response.message = ex.Message;
            response.data = ex.StackTrace;
            return StatusCode(500, response);
        }
    }

}
