using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models;

public class Pago {

    [Key]
    public int IdPago { get; set; } 

    [Required]
    public string Concepto { get;set; } = "";

    [Required]
    public DateTime FechaPago { get;set; }

    [Required]
    [Column(TypeName = ("decimal(18,2)"))]
    public decimal Importe { get; set; }

    [Required]
    public string Estado { get;set; }

    [Required]
    public int IdReserva { get; set; }
    [ForeignKey("IdReserva")]
    public Reserva? ReservaAsociada { get;set; }

    [Required]
    public int IdUsuarioCreador { get; set; }
    public int? IdUsuarioAnulador { get; set; }

}