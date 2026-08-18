using System.componentModel.DataAnnotations;

namespace Inmobilaria.Models;

public class Usuario {

    [key]
    public int idUsuario { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Contraseña { get; set; } = "";

    public string? AvatarUrl { get; set; }

    [Required]
    public string Rol { get; set; } = "";
}