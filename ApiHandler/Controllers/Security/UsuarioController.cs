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
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Listado de usuarios";
            try
            {
                response.data = await usuarioService.GetAllAsync();
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromHeader] string Authorization, [FromBody] NewUsuarioRequest userRequest)
        {

            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Usuario creado correctamente";
            try
            {
                Usuario user = new Usuario(0, userRequest.username ?? "", userRequest.perfil ?? "", true);
                response.data = await usuarioService.createAsync(user);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromHeader] string Authorization, [FromBody] EditUsuarioRequest userRequest)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Edición de usuario";
            try
            {
                Usuario? user = await usuarioService.getUsuarioByIdAsync(userRequest.id);
                if (user == null)
                {
                    response.code = "404";
                    response.message = "Usuario no encontrado";
                    return NotFound(response);
                }
                user.Username = userRequest.username ?? "";
                user.Perfil = userRequest.perfil ?? "";
                try
                {
                    user = await usuarioService.updateAsync(user);
                }
                catch (Exception ex)
                {
                    response.code = "500";
                    response.message = ex.Message;
                    response.data = ex.StackTrace;
                    return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromHeader] string Authorization, int id)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Usuario eliminado correctamente";

            try
            {
                Usuario? usuario = await usuarioService.getUsuarioByIdAsync(id);
                if (usuario == null)
                {
                    response.code = "404";
                    response.message = "Usuario no encontrado";
                    return NotFound(response);
                }
                usuario.Activo = false;
                await usuarioService.updateAsync(usuario);
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetElement([FromHeader] string Authorization, int id)
        {
            if (!jwt.ValidateJwtToken(Authorization))
            {
                return Unauthorized();
            }
            ResponseApi response = new ResponseApi();
            response.code = "000";
            response.message = "Detalle de usuario";
            try
            {
                Usuario? usuario = await usuarioService.getUsuarioByIdAsync(id);
                if (usuario == null)
                {
                    response.code = "404";
                    response.message = "Usuario no encontrado";
                    return NotFound(response);
                }
                response.data = usuario;
            }
            catch (Exception ex)
            {
                response.code = "500";
                response.message = ex.Message;
                response.data = ex.StackTrace;
                return StatusCode(500, response);
            }
            return Ok(response);
        }
    }
}