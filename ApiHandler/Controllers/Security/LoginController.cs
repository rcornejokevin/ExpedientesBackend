using Microsoft.AspNetCore.Mvc;
using ApiHandler.Models.Security;
using Support.Items;
using ResponseApi = ApiHandler.Models.Response;
using DBHandler.Service.Security;
using DBHandler.Models;
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message ?? "Solicitud inválida" : e.ErrorMessage)
                    .ToList();
                response.code = "400";
                response.message = errors.FirstOrDefault() ?? "Solicitud inválida";
                response.data = errors;
                return Ok(response);
            }
            Usuario? user = await authService.LoginSuccessAsync(request.username, request.password);
            if (user == null || user.Operativo == 0)
            {
                response.code = "999";
                response.message = "El usuario y la contraseña son incorrectos";
                return Ok(response);
            }
            if (authService.canLoginByTerminal(user, request.device) == false)
            {
                response.code = "999";
                response.message = "Se envio correo de verificación por dispositivo no reconocido";
                await authService.SendTwoFactorAsync(user, request.device);
                return Ok(response);
            }
            var token = jwt.GenerateJwtToken(request.username);
            response.code = "000";
            response.message = "Login successful";
            response.data = new { token.Token, token.Expire, user?.Perfil };
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.code = "500";
            response.message = ex.Message;
            response.data = ex.StackTrace;
            return Ok(response);
        }
    }
    [HttpPut("twofactor")]
    public async Task<IActionResult> Twofactor([FromBody] TwoFactorRequest request)
    {
        ResponseApi response = new ResponseApi();
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message ?? "Solicitud inválida" : e.ErrorMessage)
                    .ToList();
                response.code = "400";
                response.message = errors.FirstOrDefault() ?? "Solicitud inválida";
                response.data = errors;
                return Ok(response);
            }
            Usuario? user = await authService.GetUsuarioByTerminalHashAsync(request.token);
            if (user == null)
            {
                response.code = "404";
                response.message = "Solicitud de verificación no encontrada o expirada";
                return Ok(response);
            }

            if (!authService.TryExtractPendingTerminal(user, out var pendingHash, out var pendingDevice))
            {
                response.code = "400";
                response.message = "No existe dispositivo pendiente de aprobación";
                return Ok(response);
            }

            if (!string.Equals(pendingHash, request.token, StringComparison.Ordinal))
            {
                response.code = "400";
                response.message = "Token inválido para el usuario";
                return Ok(response);
            }

            Usuario updatedUser = await authService.ApprovePendingTerminalAsync(user, pendingDevice);

            var token = jwt.GenerateJwtToken(updatedUser.Username);
            response.code = "000";
            response.message = "Login successful";
            response.data = new { token.Token, token.Expire, updatedUser?.Perfil };
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.code = "500";
            response.message = ex.Message;
            response.data = ex.StackTrace;
            return Ok(response);
        }
    }

}
