using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppEnvios.Models
{
    public class Paquete
    {
        [Key]
        public int PaqueteId { get; set; }

        [Required]
        public int EnvioId { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Peso { get; set; }

        public Envio Envio { get; set; }
    }
}