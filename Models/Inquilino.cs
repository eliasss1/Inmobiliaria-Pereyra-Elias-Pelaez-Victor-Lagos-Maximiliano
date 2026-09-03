using System.ComponentModel.DataAnnotations;


namespace Inmobiliaria.Models;

public class Inquilino
{
    [Key] // este valor es para indicar que es la primary Key
    public int IdInquilino { get; set; }

    [Required]
    public string Nombre { get; set; } = "";

    [Required]
    public string Apellido { get; set; } = "";

    [Required]
    public string Dni {get; set;} = "";

    public string Telefono { get; set; } = "";
    
    [Required, EmailAddress]
    public string Email {get; set;} = "";
    public override string ToString() => $"{Nombre} {Apellido}";
}