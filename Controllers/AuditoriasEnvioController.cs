using AppEnvios.Data;
using AppEnvios.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppEnvios.Controllers
{
    [RolAutorizado("Admin")]

    public class AuditoriasEnvioController : Controller
    {
        private readonly AppDbContext _context;

        public AuditoriasEnvioController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var auditorias = await _context.AuditoriasEnvio
                .OrderByDescending(a => a.FechaAccion)
                .ToListAsync();
            return View(auditorias);
        }
    }
}