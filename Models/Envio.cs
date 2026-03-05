using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppEnvios.Models
{
    public class Envio
    {
        [Key]
        public int EnvioId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int DestinatarioId { get; set; }

        [Required]
        public int EstadoId { get; set; }

        public DateTime FechaEnvio { get; set; }

        public DateTime? FechaEntrega { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Costo { get; set; }

        public Cliente? Cliente { get; set; }
        public Destinatario? Destinatario { get; set; }

        [ForeignKey("EstadoId")]
        public EstadoEnvio? EstadoEnvio { get; set; }

        public ICollection<Paquete>? Paquetes { get; set; }
    }
}