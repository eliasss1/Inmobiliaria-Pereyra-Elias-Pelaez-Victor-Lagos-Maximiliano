namespace Inmobiliaria.Models;

public interface IRepositorioInmueble : IRepositorio<Inmueble>
{
    public IList<Inmueble> ObtenerPorPropietario(int idPropietario);
    public IList<Inmueble> ObtenerTodos();
    IList<Inmueble> Buscar(string busqueda);
    IList<Inmueble> ObtenerPorDisponibilidad(bool estado);
    IList<Inmueble> ObtenerMasReservados();
    IList<Inmueble> ObtenerSinReservas(int dias);
    IList<Inmueble> ObtenerLibresEntreFechas(DateTime inicio, DateTime fin);
}
