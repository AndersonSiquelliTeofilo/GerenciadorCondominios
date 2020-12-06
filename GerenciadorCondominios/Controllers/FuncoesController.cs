using GerenciadorCondominios.BLL.Models;
using GerenciadorCondominios.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorCondominios.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class FuncoesController : Controller
    {
        private readonly IFuncaoRepositorio _funcaoRepositorio;

        public FuncoesController(IFuncaoRepositorio funcaoRepositorio)
        {
            _funcaoRepositorio = funcaoRepositorio;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _funcaoRepositorio.PegarTodos());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Descricao,Id,Name,NormalizedName,ConcurrencyStamp")] Funcao funcao)
        {
            if (ModelState.IsValid)
            {
                await _funcaoRepositorio.AdicionarFuncao(funcao);
                TempData["NovoRegistro"] = $"Função {funcao.Name} adicionada";
                return RedirectToAction(nameof(Index));
            }
            return View(funcao);
        }

        public async Task<IActionResult> Edit(string id)
        {
            Funcao funcao = await _funcaoRepositorio.PegarPeloId(id);
            if(funcao == null)
            {
                return NotFound();
            }
            return View(funcao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Descricao,Id,Name,NormalizedName,ConcurrencyStamp")] Funcao funcao)
        {
            if (ModelState.IsValid)
            {
                await _funcaoRepositorio.Atualizar(funcao);
                TempData["Atualizacao"] = $"Função {funcao.Name} atualizada";
                return RedirectToAction(nameof(Index));
            }
            return View(funcao);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(string id)
        {
            await _funcaoRepositorio.Excluir(id);
            TempData["Exclusao"] = $"Função excluída";
            return Json("Função Excluída");
        }
    }
}
