namespace Inmobiliaria.Models;

public interface IRepositorioPago : IRepositorio<Pago>
{
    public int Alta(Pago p);


    public int Baja(int id);


    public int Modificacion(Pago p);


    public int ObtenerCantidad();


    public IList<Pago> ObtenerLista(int paginaNro = 1, int tamPagina = 10);


    public Pago? ObtenerPorId(int id);

}