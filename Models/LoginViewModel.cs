using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo electronico es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo no valido")]
    public string Email { get ; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    public string Password { get ; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; } 
}