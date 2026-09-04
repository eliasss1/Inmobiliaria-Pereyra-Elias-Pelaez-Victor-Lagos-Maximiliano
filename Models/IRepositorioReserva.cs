namespace Inmobiliaria.Models;

public interface IRepositorioReserva : IRepositorio<Reserva>
{
    public IList<Reserva> ObtenerTodos();
}