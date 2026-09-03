using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class TipoInmueble
    {
        [Key]
        public int IdTipoInmueble { get; set; }

        [Required]
        public string Nombre { get; set; } = "";

        public override string ToString() => Nombre;
    }
}