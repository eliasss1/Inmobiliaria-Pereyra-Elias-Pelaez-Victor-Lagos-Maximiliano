namespace Inmobiliaria.Models;

public interface IRepositorioReserva : IRepositorio<Reserva>
{
    public IList<Reserva> ObtenerTodos();
    public bool ExisteSolapamiento(Reserva reserva);
}