using AppEnvios.Data;
using AppEnvios.Filters;
using AppEnvios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AppEnvios.Controllers
{
    [RolAutorizado("Admin", "Cliente")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            IQueryable<Envio> query = _context.Envios
                .Include(e => e.EstadoEnvio);

            if (rol == "Cliente")
            {
                var clienteId = int.Parse(HttpContext.Session.GetString("ClienteId") ?? "0");
                query = query.Where(e => e.ClienteId == clienteId);
            }

            var envios = await query.ToListAsync();

            var vm = new DashboardViewModel
            {
                TotalEnvios = envios.Count,
                EnviosPendientes = envios.Count(e => e.EstadoEnvio?.NombreEstado == "Pendiente"),
                EnviosEnTransito = envios.Count(e => e.EstadoEnvio?.NombreEstado == "En transito"),
                EnviosEntregados = envios.Count(e => e.EstadoEnvio?.NombreEstado == "Entregado"),
                TotalClientes = rol == "Admin" ? await _context.Clientes.CountAsync() : 0,
                TotalUsuarios = rol == "Admin" ? await _context.Usuarios.CountAsync() : 0,
                IngresoTotal = envios.Sum(e => e.Costo)
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
