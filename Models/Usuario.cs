using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models;

public class Usuario {

    [Key]
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = "";

    [Required(ErrorMessage = "El correo electrónico es obligatorio"), EmailAddress(ErrorMessage = "El formato del correo electrónico es inválido")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    public string Clave { get; set; } = "";

    public string? Avatar { get; set; }

    public string Rol { get; set; } = "";
}