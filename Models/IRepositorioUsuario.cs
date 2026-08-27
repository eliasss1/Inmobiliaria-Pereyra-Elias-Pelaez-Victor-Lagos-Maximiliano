

namespace Inmobiliaria.Models;

public interface IRepositorioUsuario : IRepositorio<Usuario>
{
    public int Alta(Usuario p);

    public int Baja(int id);

    public int Modificacion(Usuario p);


    public int ObtenerCantidad();


    public IList<Usuario> ObtenerLista(int paginaNro = 1, int tamPagina = 10);


    public Usuario? ObtenerPorId(int id);
}