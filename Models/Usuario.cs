using System.ComponentModel.DataAnnotations;

namespace AppEnvios.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; }  // "Admin" o "Cliente"

        public int? ClienteId { get; set; }  // Solo para rol Cliente

        public Cliente? Cliente { get; set; }
    }
}
