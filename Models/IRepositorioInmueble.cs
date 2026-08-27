namespace Inmobiliaria.Models;

public interface IRepositorioInmueble : IRepositorio<Inmueble>
{
    public int Alta(Inmueble p);

    public int Baja(int id);

    public int Modificacion(Inmueble p);

    public int ObtenerCantidad();

    public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10);

    public Inmueble? ObtenerPorId(int id);
}
