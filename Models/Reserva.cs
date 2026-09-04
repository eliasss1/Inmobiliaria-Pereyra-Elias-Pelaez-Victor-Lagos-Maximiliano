using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Inmobiliaria.Models;

public class Reserva {

    [Key]
    public int IdReserva { get; set; }

    [Required (ErrorMessage = "La fecha de inicio es obligatoria")]
    public DateTime FechaDesde { get; set; }

    [Required (ErrorMessage = "La fecha de fin es obligatoria")]
    public DateTime FechaHasta { get; set; } 

    [Required (ErrorMessage = "El monto diario es obligatorio")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MontoPorDia { get;set; }

    public DateTime? FechaEfectivaTerminacion { get; set; }

    [Column(TypeName = " decimal(18,2)")]
    public decimal? Multa { get; set; }


    [Required (ErrorMessage = "No hay un inquilino asociado a la reserva")]
    public int IdInquilino { get; set; }
    [ForeignKey("idInquilino")]
    public Inquilino? InquilinoAsociado { get; set; }

    [Required(ErrorMessage = "No hay un inmueble asociado a la reserva")]
    public int IdInmueble { get; set; }
    [ForeignKey("idInmueble")]
    public Inmueble? InmuebleAsociado { get; set; }

    [Required(ErrorMessage = "No hay un usuario asociado a la reserva")]
    public int IdUsuarioCreador { get;set; }

    public int? IdUsuarioTerminador { get;set;}

}