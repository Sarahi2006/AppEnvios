using AppEnvios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppEnvios.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Destinatario> Destinatarios { get; set; }
        public DbSet<Envio> Envios { get; set; }
        public DbSet<EstadoEnvio> EstadosEnvio { get; set; }
        public DbSet<Paquete> Paquetes { get; set; }
        public DbSet<AuditoriaEnvio> AuditoriasEnvio { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Cliente)
                .WithMany(c => c.Envios)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Destinatario)
                .WithMany(d => d.Envios)
                .HasForeignKey(e => e.DestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Envio>()
                .HasOne(e => e.EstadoEnvio)
                .WithMany(s => s.Envios)
                .HasForeignKey(e => e.EstadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Cliente)
                .WithMany()
                .HasForeignKey(u => u.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
