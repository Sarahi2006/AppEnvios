using System.ComponentModel.DataAnnotations;

namespace AppEnvios.Models
{
    public class AuditoriaEnvio
    {
        [Key]
        public int AuditoriaId { get; set; }

        [Required]
        public string TablaAfectada { get; set; }

        public int? RegistroId { get; set; }

        [Required]
        public string Accion { get; set; } 

        public string? ValorAnterior { get; set; }

        public string? ValorNuevo { get; set; }

        [Required]
        public string UsuarioAccion { get; set; }

        [Required]
        public DateTime FechaAccion { get; set; }
    }
}