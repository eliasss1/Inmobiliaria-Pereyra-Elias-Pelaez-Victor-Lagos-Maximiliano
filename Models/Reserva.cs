using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Inmobiliaria.Models;

public class Reserva {

    [Key]
    public int idReserva { get; set; }

    [Required]
    public DateTime fechaInicio { get; set; }

    [Required]
    public DateTime fechaFin { get; set; } 

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal montoDiario { get;set; }

    public DateTime? fechaEfectivaTerminacion { get; set; }

    [Column(TypeName = " decimal(18,2)")]
    public decimal? multa { get; set; }


    [Required]
    public int IdInquilino { get; set; }
    [ForeignKey("idInquilino")]
    public Inquilino? inquilinoAsociado { get; set; }

    [Required]
    public int idInmueble { get; set; }
    [ForeignKey("idInmueble")]
    public Inquilino? inmuebleAsociado { get; set; }

    [Required]
    public int creadoPorUsuarioId { get;set; }

    public int? terminadoPorUsuarioId { get;set;}

}