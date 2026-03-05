using AppEnvios.Data;
using AppEnvios.Filters;
using AppEnvios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AppEnvios.Controllers
{
    [RolAutorizado("Admin", "Cliente")]
    public class DestinatariosController : Controller
    {
        private readonly AppDbContext _context;

        public DestinatariosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var destinatarios = await _context.Destinatarios.ToListAsync();
            return View(destinatarios);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var destinatario = await _context.Destinatarios
                .FirstOrDefaultAsync(m => m.DestinatarioId == id);

            if (destinatario == null) return NotFound();

            return View(destinatario);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Destinatario destinatario)
        {
            ModelState.Remove("Envios");

            if (!ModelState.IsValid)
                return View(destinatario);

            _context.Destinatarios.Add(destinatario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var destinatario = await _context.Destinatarios.FindAsync(id);
            if (destinatario == null) return NotFound();

            return View(destinatario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Destinatario destinatario)
        {
            if (id != destinatario.DestinatarioId) return NotFound();

            ModelState.Remove("Envios");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(destinatario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Destinatarios.Any(e => e.DestinatarioId == destinatario.DestinatarioId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(destinatario);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var destinatario = await _context.Destinatarios
                .FirstOrDefaultAsync(m => m.DestinatarioId == id);

            if (destinatario == null) return NotFound();

            return View(destinatario);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var destinatario = await _context.Destinatarios
                .Include(d => d.Envios)
                .FirstOrDefaultAsync(d => d.DestinatarioId == id);

            if (destinatario == null)
                return NotFound();

            if (destinatario.Envios.Any())
            {
                return Content("No se puede eliminar porque tiene envíos asociados.");
            }

            _context.Destinatarios.Remove(destinatario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}