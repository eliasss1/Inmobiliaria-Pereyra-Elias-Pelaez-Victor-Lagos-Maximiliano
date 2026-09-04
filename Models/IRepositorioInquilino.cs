using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Models
{
	public interface IRepositorioInquilino : IRepositorio<Inquilino>
	{
		public IList<Inquilino> ObtenerTodos();
	}
}
