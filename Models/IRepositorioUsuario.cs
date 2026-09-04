

namespace Inmobiliaria.Models;

public interface IRepositorioUsuario : IRepositorio<Usuario>
{
    public Usuario ObtenerPorEmail(string email);
}