using AppEnvios.Data;
using AppEnvios.Filters;
using AppEnvios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AppEnvios.Controllers
{
    [RolAutorizado("Admin")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Cliente)
                .ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        public IActionResult Create()
        {
            CargarClientes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario, string Password)
        {
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Cliente");

            if (!ModelState.IsValid)
            {
                CargarClientes();
                return View(usuario);
            }

            usuario.PasswordHash = HashPassword(Password);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            CargarClientes();
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario, string? Password)
        {
            if (id != usuario.UsuarioId) return NotFound();

            ModelState.Remove("PasswordHash");
            ModelState.Remove("Cliente");
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                CargarClientes();
                return View(usuario);
            }

            if (!string.IsNullOrEmpty(Password))
            {
                usuario.PasswordHash = HashPassword(Password);
            }
            else
            {
                var usuarioExistente = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UsuarioId == id);
                usuario.PasswordHash = usuarioExistente!.PasswordHash;
            }

            _context.Update(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void CargarClientes()
        {
            ViewBag.ClienteId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Clientes, "ClienteId", "Nombre");
        }

        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}