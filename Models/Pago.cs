using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobilaria.Models;

public class Pago {

    [Key]
    public int IdPago { get; set; } 

    [Required]
    public string Concepto { get;set; }

    [Required]
    public DateTime FechaPago { get;set; }

    [Required]
    [Column(TypeName = ("decimal(18,2)"))]
    public decimal Importe { get; set; }

    [Required]
    public bool Estado { get;set; }

    [Required]
    public int IdReserva { get; set; }
    [ForeignKey("IdReserva")]
    public Reserva? ReservaAsociada { get;set; }

    [Required]
    public int CreadoPorUsuarioId { get; set; }
    public int? AnuladoPorUsuarioId { get; set; }

}