using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models;

public class InmuebleImagen
{
    [Key]
    public int IdImagen { get; set; }

    [Required]
    public int IdInmueble { get; set; }

    [Required]
    public string Url { get; set; } = "";

    [ForeignKey("IdInmueble")]
    public Inmueble? InmuebleAsociado { get; set; }
}
