

namespace Inmobiliaria.Models;

public interface IRepositorioUsuario : IRepositorio<Usuario>
{
    public Usuario ObtenerPorEmail(string email);

    public int ModificarContraseña(int idUsuario, string claveHasheada);

    public string? ObtenerContraPorId(int id);

}