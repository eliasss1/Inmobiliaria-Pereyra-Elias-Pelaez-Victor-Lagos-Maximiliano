using System.ComponentModel.DataAnnotations;

namespace Inmobilaria.Models;

public class Usuario {

    [Key]
    public int idUsuario { get; set; }

    [Required]
    public string nombre { get; set; } = "";

    [Required, EmailAddress]
    public string email { get; set; } = "";

    [Required]
    public string contraseña { get; set; } = "";

    public string? avatarUrl { get; set; }

    [Required]
    public string rol { get; set; } = "";
}