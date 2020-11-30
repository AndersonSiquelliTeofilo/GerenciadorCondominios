using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GerenciadorCondominios.DAL.Repositorios
{
    public class HistoricoRecursosRepositorio : RepositorioGenerico<HistoricoRecursos>, IHistoricoRecursosRepositorio
    {
        private readonly Contexto _contexto;

        public HistoricoRecursosRepositorio(Contexto contexto) : base(contexto)
        {
            _contexto = contexto;
        }
    }
}
