using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models;

public class Usuario {

    [Key]
    public int IdUsuario { get; set; }

    [Required]
    public string Nombre { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Contraseña { get; set; } = "";

    public string? AvatarUrl { get; set; }

    [Required]
    public string Rol { get; set; } = "";
}