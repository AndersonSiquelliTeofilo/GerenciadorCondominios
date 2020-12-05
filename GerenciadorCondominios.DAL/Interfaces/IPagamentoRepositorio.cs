using GerenciadorCondominios.BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorCondominios.DAL.Interfaces
{
    public interface IPagamentoRepositorio : IRepositorioGenerico<Pagamento>
    {
        Task<IEnumerable<Pagamento>> PegarPagamentoPorUsuario(string usuarioId);
    }
}
