using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Inmobiliaria.Models;

public class Reserva {

    [Key]
    public int IdReserva { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; } 

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MontoDiario { get;set; }

    public DateTime? FechaEfectivaTerminacion { get; set; }

    [Column(TypeName = " decimal(18,2)")]
    public decimal? Multa { get; set; }


    [Required]
    public int IdInquilino { get; set; }
    [ForeignKey("idInquilino")]
    public Inquilino? InquilinoAsociado { get; set; }

    [Required]
    public int IdInmueble { get; set; }
    [ForeignKey("idInmueble")]
    public Inmueble? InmuebleAsociado { get; set; }

    [Required]
    public int CreadoPorUsuarioId { get;set; }

    public int? TerminadoPorUsuarioId { get;set;}

}