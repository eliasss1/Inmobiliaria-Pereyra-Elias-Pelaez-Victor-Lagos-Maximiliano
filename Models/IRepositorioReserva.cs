namespace Inmobiliaria.Models;

public interface IRepositorioReserva : IRepositorio<Reserva>
{
    public IList<Reserva> ObtenerTodos();
    IList<Reserva> Buscar(string busqueda);
    public bool ExisteSolapamiento(Reserva reserva);
    bool RegistrarTerminacionAnticipadaConPago(int idReserva, DateTime fechaTerminacion, decimal montoMulta, int idUsuario);
    IList<Reserva> ObtenerVigentes(DateTime? inicio, DateTime? fin);
    IList<Reserva> ObtenerTerminanEnXDias(int dias);
}