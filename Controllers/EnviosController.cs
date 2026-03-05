using AppEnvios.Data;
using AppEnvios.Filters;
using AppEnvios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AppEnvios.Controllers
{
    [RolAutorizado("Admin", "Cliente")]
    public class EnviosController : Controller
    {
        private readonly AppDbContext _context;

        public EnviosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? error)
        {
            if (error == "no_editable")
                ViewBag.Error = "Este envío ya no puede editarse porque está en proceso.";
            if (error == "no_eliminable")
                ViewBag.Error = "Este envío ya no puede eliminarse porque está en proceso.";

            var rol = HttpContext.Session.GetString("Rol");
            IQueryable<Envio> query = _context.Envios
                .Include(e => e.Cliente)
                .Include(e => e.Destinatario)
                .Include(e => e.EstadoEnvio);

            if (rol == "Cliente")
            {
                var clienteId = int.Parse(HttpContext.Session.GetString("ClienteId")!);
                query = query.Where(e => e.ClienteId == clienteId);
            }

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var envio = await _context.Envios
                .Include(e => e.Cliente)
                .Include(e => e.Destinatario)
                .Include(e => e.EstadoEnvio)
                .FirstOrDefaultAsync(m => m.EnvioId == id);

            if (envio == null) return NotFound();

            return View(envio);
        }

        public IActionResult Create()
        {
            CargarDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Envio envio)
        {
            try
            {
                ModelState.Remove("EstadoId");
                ModelState.Remove("FechaEnvio");
                ModelState.Remove("Cliente");
                ModelState.Remove("Destinatario");
                ModelState.Remove("EstadoEnvio");
                ModelState.Remove("Paquetes");

                // Forzar ClienteId si es Cliente
                var rol = HttpContext.Session.GetString("Rol");
                if (rol == "Cliente")
                {
                    var clienteId = int.Parse(HttpContext.Session.GetString("ClienteId")!);
                    envio.ClienteId = clienteId;
                    ModelState.Remove("ClienteId");
                }

                envio.FechaEnvio = DateTime.Now;

                // Buscar o crear estado Pendiente
                var estadoPendiente = await _context.EstadosEnvio
                    .FirstOrDefaultAsync(e => e.NombreEstado == "Pendiente");

                if (estadoPendiente == null)
                {
                    estadoPendiente = new EstadoEnvio { NombreEstado = "Pendiente" };
                    _context.EstadosEnvio.Add(estadoPendiente);
                    await _context.SaveChangesAsync();
                }

                envio.EstadoId = estadoPendiente.EstadoId;

                if (envio.ClienteId <= 0)
                {
                    TempData["Error"] = "Debe seleccionar un cliente válido";
                    CargarDropdowns();
                    return View(envio);
                }

                if (envio.DestinatarioId <= 0)
                {
                    TempData["Error"] = "Debe seleccionar un destinatario válido";
                    CargarDropdowns();
                    return View(envio);
                }

                if (envio.Costo <= 0)
                {
                    TempData["Error"] = "El costo debe ser mayor a 0";
                    CargarDropdowns();
                    return View(envio);
                }

                _context.Envios.Add(envio);
                await _context.SaveChangesAsync();

                // Auditoría
                var auditoria = new AuditoriaEnvio
                {
                    TablaAfectada = "Envios",
                    RegistroId = envio.EnvioId,
                    Accion = "INSERT",
                    ValorAnterior = null,
                    ValorNuevo = JsonSerializer.Serialize(new
                    {
                        envio.EnvioId,
                        envio.ClienteId,
                        envio.DestinatarioId,
                        envio.EstadoId,
                        envio.FechaEnvio,
                        envio.FechaEntrega,
                        envio.Costo
                    }),
                    UsuarioAccion = HttpContext.Session.GetString("Nombre") ?? "Sistema",
                    FechaAccion = DateTime.Now
                };
                _context.AuditoriasEnvio.Add(auditoria);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Envío creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar: " + ex.Message;
                CargarDropdowns();
                return View(envio);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var envio = await _context.Envios
                .Include(e => e.EstadoEnvio)
                .FirstOrDefaultAsync(e => e.EnvioId == id);

            if (envio == null) return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            if (rol == "Cliente")
            {
                var clienteId = int.Parse(HttpContext.Session.GetString("ClienteId")!);
                if (envio.ClienteId != clienteId) return Forbid();
                if (envio.EstadoEnvio?.NombreEstado != "Pendiente")
                    return RedirectToAction("Index", new { error = "no_editable" });
            }

            CargarDropdowns();
            return View(envio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Envio envio)
        {
            if (id != envio.EnvioId) return NotFound();

            try
            {
                ModelState.Remove("EstadoId");
                ModelState.Remove("FechaEnvio");
                ModelState.Remove("Cliente");
                ModelState.Remove("Destinatario");
                ModelState.Remove("EstadoEnvio");
                ModelState.Remove("Paquetes");

                var rol = HttpContext.Session.GetString("Rol");
                if (rol == "Cliente")
                {
                    var clienteId = int.Parse(HttpContext.Session.GetString("ClienteId")!);
                    envio.ClienteId = clienteId;
                    ModelState.Remove("ClienteId");
                }

                var envioOriginal = await _context.Envios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EnvioId == id);

                if (envioOriginal == null) return NotFound();

                envio.EstadoId = envioOriginal.EstadoId;
                envio.FechaEnvio = envioOriginal.FechaEnvio;

                if (envio.Costo <= 0)
                {
                    TempData["Error"] = "El costo debe ser mayor a 0";
                    CargarDropdowns();
                    return View(envio);
                }

                _context.Update(envio);
                await _context.SaveChangesAsync();

                var auditoria = new AuditoriaEnvio
                {
                    TablaAfectada = "Envios",
                    RegistroId = envio.EnvioId,
                    Accion = "UPDATE",
                    ValorAnterior = JsonSerializer.Serialize(new
                    {
                        envioOriginal.EnvioId,
                        envioOriginal.ClienteId,
                        envioOriginal.DestinatarioId,
                        envioOriginal.EstadoId,
                        envioOriginal.FechaEnvio,
                        envioOriginal.FechaEntrega,
                        envioOriginal.Costo
                    }),
                    ValorNuevo = JsonSerializer.Serialize(new
                    {
                        envio.EnvioId,
                        envio.ClienteId,
                        envio.DestinatarioId,
                        envio.EstadoId,
                        envio.FechaEnvio,
                        envio.FechaEntrega,
                        envio.Costo
                    }),
                    UsuarioAccion = HttpContext.Session.GetString("Nombre") ?? "Sistema",
                    FechaAccion = DateTime.Now
                };
                _context.AuditoriasEnvio.Add(auditoria);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Envío actualizado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar: " + ex.Message;
                CargarDropdowns();
                return View(envio);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var envio = await _context.Envios
                .Include(e => e.Cliente)
                .Include(e => e.Destinatario)
                .Include(e => e.EstadoEnvio)
                .FirstOrDefaultAsync(m => m.EnvioId == id);

            if (envio == null) return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            if (rol == "Cliente")
            {
                var clienteId = int.Parse(HttpContext.Session.GetString("ClienteId")!);
                if (envio.ClienteId != clienteId) return Forbid();
                if (envio.EstadoEnvio?.NombreEstado != "Pendiente")
                    return RedirectToAction("Index", new { error = "no_eliminable" });
            }

            return View(envio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var envio = await _context.Envios.FindAsync(id);
                if (envio != null)
                {
                    // Auditoría antes de eliminar
                    var auditoria = new AuditoriaEnvio
                    {
                        TablaAfectada = "Envios",
                        RegistroId = envio.EnvioId,
                        Accion = "DELETE",
                        ValorAnterior = JsonSerializer.Serialize(new
                        {
                            envio.EnvioId,
                            envio.ClienteId,
                            envio.DestinatarioId,
                            envio.EstadoId,
                            envio.FechaEnvio,
                            envio.FechaEntrega,
                            envio.Costo
                        }),
                        ValorNuevo = null,
                        UsuarioAccion = HttpContext.Session.GetString("Nombre") ?? "Sistema",
                        FechaAccion = DateTime.Now
                    };
                    _context.AuditoriasEnvio.Add(auditoria);

                    _context.Envios.Remove(envio);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Envío eliminado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private void CargarDropdowns()
        {
            var rol = HttpContext.Session.GetString("Rol");

            // Clientes
            if (rol == "Cliente")
            {
                var clienteId = int.Parse(HttpContext.Session.GetString("ClienteId")!);
                var cliente = _context.Clientes.Find(clienteId);
                if (cliente != null)
                {
                    ViewBag.ClienteId = new SelectList(new[] { cliente }, "ClienteId", "Nombre", clienteId);
                }
            }
            else
            {
                ViewBag.ClienteId = new SelectList(_context.Clientes, "ClienteId", "Nombre");
            }

            ViewBag.DestinatarioId = new SelectList(_context.Destinatarios, "DestinatarioId", "Nombre");

            ViewBag.EstadoId = new SelectList(_context.EstadosEnvio, "EstadoId", "NombreEstado");
        }
    }
}