using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace ApiWeb.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly MiDbContext _context;

        public UsuariosController(MiDbContext context)
        {
            _context = context;
        }



        [HttpGet("login")]
        public IActionResult GetLogin(string username, string cedula)
        {
            try
            {
                var user = _context.Usuarios
                    .Where(u => u.UserName == username)
                    .Where(u => u.Cedula.ToString() == cedula)
                    .Where(u => u.Estado == true);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsuariosActivos()
        {
            var query = _context.Usuarios
                .Where(u => u.Estado == true);



            var usuarios = await query
                .Select(u => new {
                    Id=u.Id,
                    Cedula=u.Cedula,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Alias = u.UserName,
                    Estado = u.Estado,
                    FechaCreacion = u.FechaCreacion.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            return Ok(usuarios);
        }


        // GET api/usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(Int64 id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // POST api/usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            usuario.FechaCreacion = DateTime.Now;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Int64 id, [FromBody] Usuario usuario)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.Apellido = usuario.Apellido;
            usuarioExistente.Cedula = usuario.Cedula;
            usuarioExistente.UserName = usuario.UserName;
            usuarioExistente.Estado = usuario.Estado;
            usuarioExistente.FechaModificacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Int64 id)
        {

            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            usuarioExistente.Estado = false;
            usuarioExistente.FechaEliminacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();

        }

        private bool UsuarioExists(Int64 id)
        {
            return _context.Usuarios.Any(u => u.Id == id);

        }
    }
}
