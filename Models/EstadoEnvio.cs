using System.ComponentModel.DataAnnotations;

namespace AppEnvios.Models
{
    public class EstadoEnvio
    {
        [Key]
        public int EstadoId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreEstado { get; set; }

        
        public ICollection<Envio> Envios { get; set; }
    }
}