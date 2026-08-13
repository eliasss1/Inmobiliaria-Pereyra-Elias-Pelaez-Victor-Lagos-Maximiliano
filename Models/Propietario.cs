using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models;

public class Propietario
{
    [Key]
    public int IdPropietario { get; set; }
    [Required]
    public string Dni { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Apellido { get; set; }
    [Required]
    public string Telefono { get; set; }
    public string Email { get; set; } = "";
	[Required(ErrorMessage = "La clave es obligatoria"), DataType(DataType.Password)]
	public string Clave { get; set; } = "";
    public override string ToString() => $"{Nombre} {Apellido}";
}