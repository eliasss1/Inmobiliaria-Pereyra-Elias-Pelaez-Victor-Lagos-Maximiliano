namespace Inmobiliaria.Models;

public interface IRepositorioPago : IRepositorio<Pago>
{
    IList<Pago> ObtenerPorReserva(int idReserva);
    int Baja(int id, int idUsuarioAnulador);
}