using AppEnvios.Data;
using AppEnvios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AppEnvios.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var hash = HashPassword(password);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hash);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }

            HttpContext.Session.SetString("UsuarioId", usuario.UsuarioId.ToString());
            HttpContext.Session.SetString("Nombre", usuario.Nombre);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            if (usuario.ClienteId.HasValue)
                HttpContext.Session.SetString("ClienteId", usuario.ClienteId.Value.ToString());

            return RedirectToAction("Index", "Home");
        }

        // GET /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET /Account/Denegado
        public IActionResult Denegado()
        {
            return View();
        }

        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}