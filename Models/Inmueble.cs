using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Inmobiliaria.Models;

public class Inmueble {

    [Key]
    public int IdInmueble { get;set; }

    [Required]
    public string Direccion { get; set; } = "";

    [Required]
    public int Cupo { get; set; } 

    [Required]
    public string Tipo { get; set; } = ""; 
    public double Latitud { get; set; }
    public double Longitud { get; set; } 

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioPorDia { get; set; } 

    [Required]
    public bool Estado { get; set; } = true; 

    
    [Required]
    public int IdPropietario { get; set; } 
    
    [ForeignKey("IdPropietario")]
    public Propietario? Dueño { get; set; }

    public override string ToString() => $"{Direccion} - Cupo: {Cupo} ({Tipo})";
}