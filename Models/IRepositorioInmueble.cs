namespace Inmobiliaria.Models;

public interface IRepositorioInmueble : IRepositorio<Inmueble>
{
    public IList<Inmueble> ObtenerPorPropietario(int idPropietario);
    
}
