using AppEnvios.Data;
using AppEnvios.Filters;
using AppEnvios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppEnvios.Controllers
{
    [RolAutorizado("Admin")]
    public class EstadosEnvioController : Controller
    {
        private readonly AppDbContext _context;

        public EstadosEnvioController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var estados = await _context.EstadosEnvio.ToListAsync();
            return View(estados);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstadoEnvio estado)
        {
            ModelState.Remove("Envios");

            if (!ModelState.IsValid)
                return View(estado);

            _context.EstadosEnvio.Add(estado);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var estado = await _context.EstadosEnvio.FindAsync(id);
            if (estado == null) return NotFound();

            return View(estado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EstadoEnvio estado)
        {
            if (id != estado.EstadoId) return NotFound();

            ModelState.Remove("Envios");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.EstadosEnvio.Any(e => e.EstadoId == estado.EstadoId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(estado);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var estado = await _context.EstadosEnvio
                .FirstOrDefaultAsync(m => m.EstadoId == id);

            if (estado == null) return NotFound();

            return View(estado);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estado = await _context.EstadosEnvio.FindAsync(id);
            if (estado != null)
            {
                _context.EstadosEnvio.Remove(estado);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}