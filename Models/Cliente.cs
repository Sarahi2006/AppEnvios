
using System.ComponentModel.DataAnnotations;

namespace AppEnvios.Models
{

    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(300)]
        public string Direccion { get; set; }

       
        public DateTime FechaRegistro { get; set; }


        public ICollection<Envio>? Envios { get; set; }
    }
}