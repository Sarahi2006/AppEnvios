using System.ComponentModel.DataAnnotations;

namespace AppEnvios.Models
{
    public class Destinatario
    {
        [Key]
        public int DestinatarioId { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(300)]
        public string Direccion { get; set; }

        [Required]
        [StringLength(100)]
        public string Ciudad { get; set; }

        [Required]
        [StringLength(100)]
        public string Pais { get; set; }

        public ICollection<Envio> Envios { get; set; }
    }
}