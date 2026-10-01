namespace Inmobiliaria.Models;

public interface IRepositorioInmueble : IRepositorio<Inmueble>
{
    public IList<Inmueble> ObtenerPorPropietario(int idPropietario);
    public IList<Inmueble> ObtenerTodos();
    IList<Inmueble> Buscar(string busqueda);
    IList<Inmueble> ObtenerPorDisponibilidad(bool estado, int? idPropietario = null);
    IList<Inmueble> ObtenerMasReservados(int? idPropietario = null);
    IList<Inmueble> ObtenerSinReservas(int dias, int? idPropietario = null);
    IList<Inmueble> ObtenerLibresEntreFechas(DateTime inicio, DateTime fin, int? idPropietario = null);
}

