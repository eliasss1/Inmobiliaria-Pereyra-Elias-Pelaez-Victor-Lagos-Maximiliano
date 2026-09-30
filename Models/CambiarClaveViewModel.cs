
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models;

public class CambiarClaveViewModel
{
    [Required(ErrorMessage = "La contraseña actual es obligatoria")]
    [DataType(DataType.Password)]
    public string ClaveActual { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
    [DataType(DataType.Password)]
    public string ClaveNueva { get; set; } = string.Empty;
}