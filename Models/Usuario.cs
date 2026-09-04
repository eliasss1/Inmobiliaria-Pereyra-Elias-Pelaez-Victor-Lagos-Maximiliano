using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models;

public class Usuario {

    [Key]
    public int IdUsuario { get; set; }

    [Required]
    public string Nombre { get; set; } = "";

    [Required]
    public string Apellido { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Clave { get; set; } = "";

    public string? Avatar { get; set; }

    public string Rol { get; set; } = "";
}