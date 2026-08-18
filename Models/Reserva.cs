using System.componentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Inmobilaria.Models;

public class Reserva {

    [key]
    public int idReserva { get; set; }

    [Required]
    public DateTime fechaInicio { get; set; } = "";

    [Required]
    public DateTime FechaFin { get; set; } = "";

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MontoDiario { get;set; }

    public DateTime? FechaEfectivaTerminacion { get; set; }

    [Column(TypeName = " decimal(18,2)")]
    public decimal? Multa { get; set; }


    [Required]
    public int IdInquilino { get; set; }
    [ForeignKey("IdInquilino")]
    public Inquilino? InquilinoAsociado { get; set; }

    [Required]
    public int IdInmueble { get; set; }
    [ForeignKey("IdInquilino")]
    public Inquilino? InmuebleAsociado { get; set; }

    [Required]
    public int CreadoPorUsuarioId { get;set; }

    public int? TerminadoPorUsuarioId { get;set;}

}