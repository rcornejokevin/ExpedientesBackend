using DBHandler.Service.Security;
using ResponseApi = ApiHandler.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Support.Items;
using ApiHandler.Models.Security;
using DBHandler.Models;
namespace ApiHandler.Controllers.Security
{
    [Route("usuario")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly UsuarioService usuarioService;
        private readonly Jwt jwt;
        public UsuarioController(UsuarioService usuarioService, Jwt jwt)
        {
            this.usuarioService = usuarioService;
            this.jwt = jwt;
        }
        [HttpGet("list")]
        public async Task<IActionResult> List([FromHeader] string Authorization)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Listado de usuarios";
            try
            {
                var usuarios = await usuarioService.GetAllAsync();
                response.data = usuarios.Select(u => new
                {
                    id = u.Id,
                    username = u.Username,
                    perfil = u.Perfil,
                    operativo = u.Operativo == 1 ? true : false,
                    activo = u.Activo == 1 ? true : false
                });
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            return Ok(response);
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewUsuarioRequest userRequest)
        {

            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
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
            response.code = "000";
            response.message = "Usuario creado correctamente";
            try
            {
                Usuario user = new Usuario(0, userRequest.username ?? "", userRequest.perfil ?? "", 1)
                {
                    Operativo = userRequest.operativo ? 1 : 0
                };
                response.data = await usuarioService.createAsync(user);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            return Ok(response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditUsuarioRequest userRequest)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
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
            response.code = "000";
            response.message = "Edición de usuario";
            try
            {
                Usuario? user = await usuarioService.getUsuarioByIdAsync(userRequest.id);
                if (user == null)
                {
                    response.code = "404";
                    response.message = "Usuario no encontrado";
                    return Ok(response);
                }
                user.Username = userRequest.username ?? "";
                user.Perfil = userRequest.perfil ?? "";
                user.Operativo = userRequest.operativo ? 1 : 0;
                try
                {
                    user = await usuarioService.updateAsync(user);
                }
                catch (Exception ex)
                {
                    response.code = "500";
                    response.message = ex.Message;
                    response.data = ex.StackTrace;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromHeader] string Authorization, int id)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Usuario eliminado correctamente";

            try
            {
                Usuario? usuario = await usuarioService.getUsuarioByIdAsync(id);
                if (usuario == null)
                {
                    response.code = "404";
                    response.message = "Usuario no encontrado";
                    return Ok(response);
                }
                usuario.Activo = 0;
                await usuarioService.updateAsync(usuario);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetElement([FromHeader] string Authorization, int id)
        {
            ResponseApi response = new ResponseApi();
            if (!jwt.ValidateJwtToken(Authorization))
            {
                response.code = "401";
                response.message = "Unauthorized";
                return Ok(response);
            }
            response.code = "000";
            response.message = "Detalle de usuario";
            try
            {
                Usuario? usuario = await usuarioService.getUsuarioByIdAsync(id);
                if (usuario == null)
                {
                    response.code = "404";
                    response.message = "Usuario no encontrado";
                    return Ok(response);
                }
                response.data = new
                {
                    id = usuario.Id,
                    username = usuario.Username,
                    perfil = usuario.Perfil,
                    operativo = usuario.Operativo == 1 ? true : false,
                    activo = usuario.Activo == 1 ? true : false
                };
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return Ok(response);
            }
            return Ok(response);
        }
    }
}
